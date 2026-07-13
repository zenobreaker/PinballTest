using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{
    [SerializeField] private float laserDuration = 0.5f; // 이펙트 유지 시간
    [SerializeField] private Vector2 boxSize = new Vector2(20f, 1f); // 가로로 긴 타격 박스 크기
    [SerializeField] private LayerMask enemyLayer;


    // 현재 화면에 켜져 있는 모든 레이저를 관리하는 리스트
    private static List<LaserEffect> activeLasers = new List<LaserEffect>();

    // Y축으로 위아래 5씩, 총합 10의 마진 안에 다른 레이저가 있는지 검사
    public static bool IsLaserActiveNear(float targetY, float margin = 5f)
    {
        foreach (var laser in activeLasers)
        {
            // 켜져 있는 레이저와 새 타겟의 Y축 거리 차이 계산
            if (Mathf.Abs(laser.transform.position.y - targetY) <= margin)
            {
                return true; // 마진 안에 이미 레이저가 있음!
            }
        }
        return false;
    }


    private void OnDisable()
    {
        activeLasers.Remove(this);
        ObjectPooler.ReturnToPool(gameObject);
    }

    public void FireLaser(DamageEvent damage, Character owner)
    {
        activeLasers.Add(this);

        // 타격 판정 (BoxCast를 양쪽으로 넓게 펼쳐서 스캔)
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