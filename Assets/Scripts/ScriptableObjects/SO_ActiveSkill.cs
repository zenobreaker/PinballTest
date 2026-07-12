using UnityEngine;

// --- [액티브 스킬 SO] ---
[CreateAssetMenu(fileName = "SO_ActiveSkill", menuName = "Scriptable Objects/Skill/Active Skill")]
public class SO_ActiveSkillData : SO_SkillData
{
    public BallType ballType; // 파이어, 아이스 등

    public override Skill CreateSkill()
    {
        return new ActiveSkill(this);
    }
}
