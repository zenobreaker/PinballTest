using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{

    private Dictionary<int, Skill> ownedSkills = new();

    [SerializeField]
    private List<ActiveSkill> activeSkills = new();
    
    [SerializeField]
    private List<PassiveSkill> passiveSkills = new();


    public void AcquireSkill(Skill skill)
    {
        // 이미 보유 중이면 레벨업, 아니면 추가
        // 스킬 리스트에서 찾아 레벨++ 후 ApplyEffect 호출
        if (ownedSkills.TryGetValue(skill.SkillID, out var owned))
        {
            owned.LevelUp();
        }
        else
        {
            RegisterSkill(skill);

            skill.SetLevel(1);
        }
        skill.ApplyEffect();

    }

    public void RegisterSkill(Skill skill)
    {
        ownedSkills.Add(skill.SkillID, skill); 

        BallManager.Instance.ReplaceOrUpgradeBall(skill as ActiveSkill);
    }
}