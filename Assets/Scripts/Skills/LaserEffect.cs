using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class LaserEffect : MonoBehaviour
{
    [SerializeField] private float laserDuration = 0.5f; // 이펙트 유지 시간
    [SerializeField] private Vector2 boxSize = new Vector2(20f, 1f); // 가로로 긴 타격 박스 크기
    [SerializeField] private LayerMask enemyLayer;

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    public void FireLaser(DamageEvent damage, Character owner)
    {
        // 1. 타격 판정 (BoxCast를 양쪽으로 넓게 펼쳐서 스캔)
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.OnDamage(owner, null, hit.transform.position, damage);
                Debug.Log($"[LaserEffect] {hit.name} 가로줄 관통 데미지 {damage.BaseDamage} 타격!");
            }
        }

        // 2. 이펙트 출력 후 풀로 자동 반환
        ReturnToPoolAfterDelay().Forget();
    }

    private async UniTaskVoid ReturnToPoolAfterDelay()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(laserDuration));
        gameObject.SetActive(false);
    }

    // 타격 범위 시각화 (에디터 전용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}