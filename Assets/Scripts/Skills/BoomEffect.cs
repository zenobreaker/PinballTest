using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    private float explosionDamage;
    private Character owner;
    private DamageEvent damageEvent;
    private CircleCollider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<CircleCollider2D>();
    }

    private void OnDisable()
    {
        CancelInvoke();
        ObjectPooler.ReturnToPool(gameObject);
    }

    // LastMatchPassive에서 폭발을 생성할 때 호출해줄 초기화 함수
    public void Init(float damage, Character owner)
    {
        explosionDamage = damage;
        this.owner = owner;

        // 폭발 데미지 세팅
        damageEvent = new DamageEvent(damage);

        // 풀에서 다시 꺼냈을 때 트리거를 확실하게 켜줍니다.
        if (myCollider != null)
            myCollider.enabled = true;

        // 0.5초 뒤에 풀로 반환 (파티클이나 애니메이션 길이에 맞춰 수정하세요!)
        Invoke(nameof(ReturnToPool), 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적과 부딪혔을 때만 데미지 처리
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<IDamagable>(out var damagable))
            {
                // 타격 지점 계산 및 데미지 전달
                Vector2 hitPoint = collision.ClosestPoint(transform.position);
                damagable.OnDamage(owner, null, hitPoint, damageEvent);

                Debug.Log($"[BoomEffect] 폭발이 {collision.name}에게 {explosionDamage} 데미지 적중!");
            }
        }
    }

    private void ReturnToPool()
    {
        // 풀로 돌아가기 전에 콜라이더를 꺼서 엉뚱한 적중을 막습니다.
        if (myCollider != null)
            myCollider.enabled = false;

        // 오브젝트 풀 매니저 방식에 맞게 반환 (예: ObjectPooler.ReturnToPool)
        gameObject.SetActive(false);
    }
}