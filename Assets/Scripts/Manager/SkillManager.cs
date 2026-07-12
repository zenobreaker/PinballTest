using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{

    private Dictionary<int, Skill> ownedSkills = new();

    private List<ActiveSkill> activeSkills = new();
    
    private List<PassiveSkill> passiveSkills = new();


    [SerializeField] private List<SO_SkillData> skillDatas;

    protected override void Awake()
    {
        if (skillDatas == null) return; 

        foreach(var data in skillDatas)
        {
            Skill skill = data.CreateSkill();
            if (skill is ActiveSkill active)
                activeSkills.Add(active);
            else if (skill is PassiveSkill passive)
                passiveSkills.Add(passive); 
        }
    }

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
    }

    // 💡 3개의 랜덤한 스킬 선택지를 반환하는 함수
    public List<Skill> GetRandomAvailableSkills()
    {
        List<Skill> availablePool = new List<Skill>();

        // 볼 매니저로부터 볼 풀에 여유가 있는지 확인
        bool hasBallSpace = BallManager.Instance.GetAvailableBallPool();

        // 1. 액티브 스킬 필터링
        foreach (var active in activeSkills)
        {
            if (ownedSkills.TryGetValue(active.SkillID, out var owned))
            {
                // 이미 보유 중인 경우: 만렙(3레벨) 미만일 때만 업그레이드 용도로 추가
                if (owned.SkillLevel < 3)
                {
                    availablePool.Add(active);
                }
            }
            else
            {
                // 미보유 중인 경우: 볼 풀에 빈 공간이 있을 때만 신규 획득 용도로 추가
                if (hasBallSpace)
                {
                    availablePool.Add(active);
                }
            }
        }

        // 2. 패시브 스킬 필터링
        foreach (var passive in passiveSkills)
        {
            if (ownedSkills.TryGetValue(passive.SkillID, out var owned))
            {
                // 이미 보유 중인 경우: 만렙(3레벨) 미만일 때만 추가
                if (owned.SkillLevel < 3)
                {
                    availablePool.Add(passive);
                }
            }
            else
            {
                // 미보유 중인 패시브는 조건 없이 추가 
                availablePool.Add(passive);
            }
        }

        // 3. 필터링된 풀에서 중복 없이 랜덤으로 최대 3개 추출
        List<Skill> selectedSkills = new List<Skill>();
        int pickCount = Mathf.Min(3, availablePool.Count); // 뽑을 수 있는 스킬이 3개 미만인 경우 방어

        for (int i = 0; i < pickCount; i++)
        {
            int randomIndex = Random.Range(0, availablePool.Count);
            selectedSkills.Add(availablePool[randomIndex]);

            // 중복 추출을 막기 위해 뽑힌 스킬은 풀에서 제거
            availablePool.RemoveAt(randomIndex);
        }

        return selectedSkills;
    }
}