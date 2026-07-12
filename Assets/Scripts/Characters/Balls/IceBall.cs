using UnityEngine;

public class IceBall : Ball
{
    protected override void ApplySpecialEffect(GameObject target)
    {
        // 1. 스킬 데이터가 주입되지 않았다면 무시
        if (skillData == null) return;

        // 확률 체크 (예: effectChance가 0.3f 이면 30% 확률)
        if (Random.value <= skillData.effectChance)
        {
            if (target.TryGetComponent<EffectComponent>(out var effectComp))
            {

                FreezeEffect freeze = new FreezeEffect(
                    id: "Freeze_Iceball",
                    desc: "아이스볼 빙결 효과",
                    duration: skillData.effectDuration,
                    param1: skillData.effectValue,
                    param2: skillData.effectValue
                );

                effectComp.ApplyEffect(freeze, target, this.gameObject);
                Debug.Log($"[IceBall] {target.name} 빙결! (지속: {skillData.effectDuration}초)");
            }
        }
    }
}
