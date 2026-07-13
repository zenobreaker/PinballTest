using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LaserBall : Ball
{
    [Header("레이저 설정")]
    [SerializeField] private string laserEffectTag = "LaserEffect"; // 가로줄 레이저 프리팹 이름
    [SerializeField] private float laserMargin = 5f;
    protected override void ApplySpecialEffect(GameObject target)
    {
        if (skillData == null) return;

        float targetY = target.transform.position.y;

        // 1. 타겟의 Y 위치 위아래(margin) 범위에 이미 레이저가 있다면 취소
        if (LaserEffect.IsLaserActiveNear(targetY, laserMargin))
            return;

        // 2. X는 0(맵 중앙), Y는 타겟 위치로 셋팅
        Vector3 spawnPos = new Vector3(0f, targetY, 0f);

        // 적중한 위치(타겟의 위치)에 레이저 이펙트 스폰
        ObjectPooler.DeferredSpawnWithCallback(laserEffectTag, spawnPos, (obj) =>
        {
            if (obj.TryGetComponent<LaserEffect>(out var laser))
            {
                // 레이저 이펙트에게 데미지(effectValue: 7, 11, 15) 전달 후 격발

                DamageEvent de = new DamageEvent
                (skillData.effectValue, 
                damageEvent.isCrit); 

                laser.FireLaser(de, this.owner);
            }
            ObjectPooler.FinishSpawn(obj);
        });
    }
}