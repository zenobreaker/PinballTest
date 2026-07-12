using UnityEngine;

// 스킬 사용 시 처리 이벤트 
public class SkillUseEvent
{
    public int SkillID;
    public string SkillName;
    public GameObject Owner; 
}

public abstract class PassiveSkill
    : Skill
{
    protected GameObject owner;

    public PassiveSkill() : base()
    {
    }
    public PassiveSkill(int skillID, string skillName, string skillDesc, Sprite skillIcon)
        : base(skillID, skillName, skillDesc, skillIcon )
    {

    }

    public PassiveSkill(SO_SkillData data)
        : base(data)
    {

    }


    public virtual void OnAcquire(GameObject owner) { }
    public virtual void OnApplyStaticEffect(StatusComponent status) { }
    public virtual void OnChangedLevel (int newLevel) { }
    public virtual void OnLose() { }
    public virtual void OnUpdate(float dt) { }

    
    public virtual void OnHit(GameObject target) { }
    public virtual void OnBallBounce(Ball ball) { }
    public virtual void OnDamaged(DamageData damageData) { }
    public virtual void OnKill(GameObject target) { }
}