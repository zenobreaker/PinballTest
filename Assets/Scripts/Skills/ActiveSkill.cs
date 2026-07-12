using UnityEngine;

public class ActiveSkill : Skill
{
    public BallType BallType;

    public ActiveSkill(SO_ActiveSkillData data)
        : base (data)
    {
        BallType = data.ballType;
    }

    public override void ApplyEffect()
    {
        BallManager.Instance.SafeInvoke(v => v.ReplaceOrUpgradeBall(BallType, skillLevel));
    }
}
