using UnityEngine;


// --- [패시브 스킬 SO] ---
public enum PassiveType
{
    NormalBallDamageUp, // 따뜻한 양철 심장
    WallBounceDamageUp, // 마법 거울
    FrontHitCritUp,     // 자수정 단검
    BackHitCritUp,      // 에메랄드 단검
    ExplodeOnDeath      // 마지막 성냥
}

[CreateAssetMenu(fileName = "SO_PassiveSkill", menuName = "Scriptable Objects/Skill/Passive Skill")]
public class SO_PassiveSkillData : SO_SkillData
{
    public PassiveType passiveType;

    public override Skill CreateSkill()
    {
        return new PassiveSkill(this);
    }
}