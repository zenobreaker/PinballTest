using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private float currentSpeed;
    private Vector2 lastVelocity; // 충돌 직전의 방향을 기억하기 위한 변수

    private bool isPierce;

    private Collider2D myCollider;
    private Character owner;
    private DamageEvent damageEvent;

    private void Awake()
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

    private void OnDisable()
    {
        // 공이 비활성화될 때 풀로 돌아가도록 처리
        ObjectPooler.ReturnToPool(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int hitLayer = collision.gameObject.layer;

        // 1. 벽(Wall) 레이어 충돌: 정확한 입사각/반사각 계산
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
        }
        // 2. 적(Enemy) 충돌: 데미지 처리 및 반사
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적 충돌! 데미지 처리");

            // 데미지 처리 로직 연결
            Vector2 hitpoint = myCollider.SafeInvoke(v=>v.ClosestPoint(collision.gameObject.transform.position));
            DealDamage(collision.gameObject, hitpoint);

            // 적과 부딪혔을 때도 벽처럼 튕겨나가길 원한다면 아래 주석을 해제하세요.
            Vector2 surfaceNormal = collision.contacts[0].normal;
            rb.linearVelocity = Vector2.Reflect(lastVelocity.normalized, surfaceNormal) * currentSpeed;
        }
        // 3. 바닥(DeadZone) 레이어 충돌: 회수 처리
        else if (hitLayer == LayerMask.NameToLayer("DeadZone"))
        {
            ReturnBall();
        }
    }

    // 바닥이 Collider가 아닌 Trigger(Is Trigger 체크)로 설정되어 있을 경우를 대비한 방어 코드
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("DeadZone"))
        {
            ReturnBall();
        }
    }

    // 공 회수 로직 통합
    private void ReturnBall()
    {
        // 매니저에 회수 카운트 전달
        if (BallManager.Instance != null)
        {
            BallManager.Instance.OnBallReturned();
        }

        // 비활성화 (OnDisable이 호출되면서 풀로 돌아감)
        gameObject.SetActive(false);
    }

    public void SetUpData(Character owner, BallData ballData)
    {
        if (owner == null || ballData == null) return;

        this.owner = owner;
        damageEvent = ballData?.damageData.GetMyDamageEvent(owner.Status);
    }

    protected void DealDamage(GameObject target, Vector2 hitPoint)
    {
        if (target.TryGetComponent<IDamagable>(out var damage))
        {
            damage?.OnDamage(owner, null, hitPoint, damageEvent);
        }
        //OnTargetHitEvent?.Invoke(target, hitPoint);
    }
}