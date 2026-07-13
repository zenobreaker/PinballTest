using System;
using Unity.VisualScripting;
using UnityEngine;
public enum BallReturnPolicy
{
    ReturnToManager,   // 일반 공
    DestroyOnDeadZone, // 조각볼
    IgnoreDeadZone     // 필요하면 사용
}
public class Ball : MonoBehaviour
{

    [SerializeField]
    protected BallReturnPolicy returnPolicy = BallReturnPolicy.ReturnToManager;
    protected  Rigidbody2D rb;
    protected float currentSpeed;
    protected  Vector2 lastVelocity; // 충돌 직전의 방향을 기억하기 위한 변수

    protected SkillLevelData skillData;

    protected  Collider2D myCollider;
    protected  Character owner;
    protected DamageEvent damageEvent;
    protected BallRuntimeData runtimeData;

    public event Action<GameObject> OnHitEnemy;
    public event Action OnBounceWall;
    public event Action OnReturn;

    // 비행 중 실시간으로 누적되는 데미지 배율 (마법 거울 패시브용)
    private float currentFlightDamageAmp = 1.0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        int ballLayer = LayerMask.NameToLayer("Ball");
        int azLayer = LayerMask.NameToLayer("AttackZone");
        Physics2D.IgnoreLayerCollision(ballLayer, ballLayer, true);
        Physics2D.IgnoreLayerCollision(ballLayer, azLayer, true);
        myCollider = GetComponent<Collider2D>();
    }


    // ShootComponent에서 공을 풀에서 꺼낸 직후 호출할 함수
    public void Launch(Vector2 direction, float speed)
    {
        currentSpeed = speed;
        currentFlightDamageAmp = 1.0f;

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * currentSpeed;
            lastVelocity = rb.linearVelocity; // 초기 속도 백업
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // 2D 물리 연산 중 마찰이나 미세한 오차로 속도가 줄어드는 것을 방지 (등속 운동 유지)
        if (rb.linearVelocity != Vector2.zero)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }

        // 다음 프레임의 충돌 반사각 계산을 위해, 현재 궤적(속도)을 계속 업데이트하여 기억해 둡니다.
        lastVelocity = rb.linearVelocity;
    }

    protected virtual void OnDisable()
    {
        // 공이 비활성화될 때 풀로 돌아가도록 처리
        ObjectPooler.ReturnToPool(gameObject);
    }

    protected  virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적 충돌! 데미지 처리");

            // 데미지 처리 로직 연결
            //Vector2 hitpoint = myCollider.SafeInvoke(v => v.ClosestPoint(collision.gameObject.transform.position));
            DealDamage(collision.gameObject, this.transform.position, collision);

            // 자식 클래스(파이어볼 등)의 특수 효과 발동을 위한 가상 함수 호출
            ApplySpecialEffect(collision.gameObject);

            // 외부 패시브 스킬들을 위한 이벤트 발생
            OnHitEnemy?.Invoke(collision.gameObject);
            // 적중 후 '다음 타격 데미지 증가' 배율 초기화
            currentFlightDamageAmp = 1.0f;


            // 적과 부딪혔을 때도 벽처럼 튕겨나가길 원한다면 아래 주석을 해제하세요.
            Vector2 surfaceNormal = collision.contacts[0].normal;
            rb.linearVelocity = Vector2.Reflect(lastVelocity.normalized, surfaceNormal) * currentSpeed;
        }
        else
            Bounce(collision);
    }

    // 바닥이 Collider가 아닌 Trigger(Is Trigger 체크)로 설정되어 있을 경우를 대비한 방어 코드
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("DeadZone"))
        {
            ReturnBall();
        }
    }

    protected void Bounce(Collision2D collision)
    {
        int hitLayer = collision.gameObject.layer;

        if (hitLayer == LayerMask.NameToLayer("Wall"))
        {
            // 충돌 표면의 법선 벡터(Normal)를 가져옵니다.
            Vector2 surfaceNormal = collision.contacts[0].normal;

            // 물리 엔진에 의존하지 않고, 직전 궤적(lastVelocity)을 기준으로 완벽한 반사각을 덮어씌웁니다.
            Vector2 reflectedDir = Vector2.Reflect(lastVelocity.normalized, surfaceNormal);

            // 무한 루프 방지 로직(완전 수평 / 수직 왕복 방지)
            float minAngleThreshold = 0.15f;

            // Y축(상하)으로 너무 평행하게 움직이는 경우
            if (Mathf.Abs(reflectedDir.y) < minAngleThreshold)
            {
                // 완전히 0이 되어 상단에 갇힌 경우 강제로 아래(-1)로 향하게 함
                float signY = (reflectedDir.y == 0f) ? -1f : Mathf.Sign(reflectedDir.y);
                reflectedDir.y = signY * minAngleThreshold;
            }

            // X축(좌우)으로 너무 평행하게 움직이는 경우
            if (Mathf.Abs(reflectedDir.x) < minAngleThreshold)
            {
                float signX = (reflectedDir.x == 0f) ? 1f : Mathf.Sign(reflectedDir.x);
                reflectedDir.x = signX * minAngleThreshold;
            }

            // 각도를 비틀었으므로 다시 정규화(Normalize) 해줍니다.
            reflectedDir.Normalize();


            rb.linearVelocity = reflectedDir * currentSpeed;
            InvokeBounceWall();
        }
        else if (hitLayer == LayerMask.NameToLayer("DeadZone"))
        {
            ReturnBall();
        }
    }


    protected void InvokeBounceWall()
    {
        // 마법 거울 패시브: 벽에 튕길 때마다 데미지 배율 누적
        if (runtimeData != null && runtimeData.BounceBonusDamageAmp > 0)
        {
            currentFlightDamageAmp += runtimeData.BounceBonusDamageAmp;
            Debug.Log($"벽 바운스! 다음 타격 데미지 배율: {currentFlightDamageAmp}x");
        }

        OnBounceWall?.Invoke();
    }


    // 자식 클래스(파이어볼, 아이스볼 등)에서 오버라이딩하여 고유 능력을 구현할 함수
    protected virtual void ApplySpecialEffect(GameObject target)
    {
        // 기본 Normal Ball은 아무 특수 효과가 없습니다.
    }

    // 공을 풀에서 꺼낼 때 매니저가 스킬 데이터를 주입해주는 함수
    public void SetSkillData(SkillLevelData data)
    {
        this.skillData = data;
    }

    // 공 회수 로직 통합
    protected  void ReturnBall()
    {
        switch (returnPolicy)
        {
            case BallReturnPolicy.ReturnToManager:
                BallManager.Instance.SafeInvoke(v => v.OnBallReturned());
                OnReturn?.Invoke();
                gameObject.SetActive(false);
                break;

            case BallReturnPolicy.DestroyOnDeadZone:
                gameObject.SetActive(false);
                break;

            case BallReturnPolicy.IgnoreDeadZone:
                break;
        }
        
        // 비활성화 (OnDisable이 호출되면서 풀로 돌아감)
        gameObject.SetActive(false);
    }

    public void SetUpData(Character owner, BallData ballData, BallRuntimeData runtime = null)
    {
        if (owner == null || ballData == null) return;

        this.owner = owner;
        // 여기선 데미지만 축적 
        DamageData damageData = ballData.damageData.Clone();

        if (runtime != null)
        {
            runtimeData = runtime;
            if (ballData.ballType == BallType.Normal)
                damageData.baseDamage *= runtime.NormalBallDamageAmp;

        }

        damageEvent = damageData.GetMyDamageEvent(owner.Status);
    }

    private void ApplyCollisionBonus(Collision2D target, ref DamageEvent evt)
    {
        if (runtimeData == null)
            return;

        if (IsFrontHit(target))
        {
            evt.ExtraCritChance += runtimeData.FrontHitCritChanceBonus;
        }

        if (IsBackHit(target))
        {
            evt.ExtraCritChance += runtimeData.BackHitCritChanceBonus;
        }
    }


    protected void DealDamage(GameObject target, Vector2 hitPoint, Collision2D collision)
    {
        if (target.TryGetComponent<IDamagable>(out var damage))
        {
            DamageEvent currentHitEvent = this.damageEvent.Clone();

            ApplyCollisionBonus(collision, ref currentHitEvent);

            currentHitEvent.DamageAmp = currentFlightDamageAmp;

            damage?.OnDamage(owner, this, hitPoint, currentHitEvent);

        }
    }

    private bool IsFrontHit(Collision2D collision)
    {
        return collision.GetContact(0).point.y <
               collision.collider.bounds.center.y;
    }

    private bool IsBackHit(Collision2D collision)
    {
        return !IsFrontHit(collision);
    }
}