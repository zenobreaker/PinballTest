using UnityEngine;

public class FreezeEffect : BaseEffect
{
    private StatModifier modifier;
    private float amplipoint = 0.0f;
    private StatusComponent status; 

    public FreezeEffect(string id, string desc, float duration, 
        float param1 = 0.0f, float param2 = 0.0f)
        : base(id, desc, duration)
    {
        Type = EffectType.DEBUFF;
        
        amplipoint = param2;

        this.modifier = ModifierFactory.CreateStatModifier(StatusType.MOVESPEED,
           param1 * -1.0f, ModifierValueType.PERCENT) ;
        Actions.Add(new ApplyStatModifierAction(modifier));
    }

    public override void OnApply(GameObject owner, GameObject appliedBy)
    {
        base.OnApply(owner, appliedBy);
        status = owner.GetComponent<StatusComponent>();

        Debug.Log($"{owner.name}에 빙결 적용 받는 피해 N% 증가 및 이동속도 감소");
    }

    public float GetDamageIncrease() => amplipoint;

    public override void OnRemove()
    {
        status.SafeInvoke(v => v.RemoveBuff(modifier));
        base.OnRemove();
    }
}
