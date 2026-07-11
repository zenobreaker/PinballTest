using UnityEngine;

public class HatredEffect
    : DoTStatusEffect
{
    public HatredEffect(string id, string desc, float duration, float tick = 0, float power =0) 
        : base(id, desc, duration, tick)
    {
        Type = EffectType.DEBUFF;
        Triggers.Add(new PeriodicTickTrigger(tickInterval));
    }

    public override BuffStackPolicy StackPolicy => BuffStackPolicy.IGNOREIFEXSIST;
}
