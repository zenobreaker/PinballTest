using UnityEngine;

public class FireBall : Ball
{
    protected override void ApplySpecialEffect(GameObject target)
    {
        // 1. 스킬 데이터가 주입되지 않았다면 무시
        if (skillData == null) return;

        // 2. 부딪힌 적에게 EffectComponent가 있는지 확인
        if (target.TryGetComponent<EffectComponent>(out var effectComp))
        { 
            float tickInterval = 0.5f; // 1초마다 데미지 적용

            var effect = EffectFactory.CreateDotStatusEffect(id: "Burn",
                desc: "파이어볼 화상 피해",
                duration: skillData.effectDuration,
                tick: tickInterval,
                power: skillData.effectValue);

            if(effect is BurnEffect burn)
            {
                burn.MaxStack = skillData.maxStack;
            }

            // 4. 컴포넌트에 디버프 적용 (StackPolicy에 따라 알아서 중첩 처리됨)
            effectComp.ApplyEffect(effect, target, this.gameObject);

            Debug.Log($"[FireBall] {target.name}에게 화상 부여! (초당 {skillData.effectValue} 피해)");
        }
    }
}
