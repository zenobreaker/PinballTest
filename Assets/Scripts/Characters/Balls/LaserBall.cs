using Unity.VisualScripting;
using UnityEngine;

public class LaserBall : Ball
{
    [Header("레이저 설정")]
    [SerializeField] private string laserEffectTag = "LaserEffect"; // 가로줄 레이저 프리팹 이름

    protected override void ApplySpecialEffect(GameObject target)
    {
        if (skillData == null) return;

        // 적중한 위치(타겟의 위치)에 레이저 이펙트 스폰
        ObjectPooler.DeferredSpawnWithCallback(laserEffectTag, target.transform, (obj) =>
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