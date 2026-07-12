using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public abstract class Skill
{
    protected int skillID;
    public int SkillID { get { return skillID; } }
    protected string skillName;
    public string Name { get { return skillName; } }
    protected string skillDescription;
    public string Description { get { return skillDescription; } }
    protected int skillLevel;
    public int SkillLevel { get { return skillLevel; } }

    public List<SkillLevelData> LevelDatas { get; protected set; }

    public Sprite Icon { get; private set; }

    public Skill()
    {
        skillLevel = 1;
    }

    public Skill(int id, string name, string desc, Sprite icon)
    {
        skillLevel = 1; 

        skillID = id;
        skillName = name;
        skillDescription = desc;
        Icon = icon;
    }

    public Skill(SO_SkillData skillData)
        : this()
    {
        skillID = skillData.id;
        skillName = skillData.skillName;
        Icon = skillData.skillImage;

        LevelDatas = skillData.levelDatas;
    }

    public virtual void SetLevel(int level)
    {
        skillLevel = level;
    }

    public string GetDesc(int level)
    {
        // 1. 레벨을 인덱스(0부터 시작)로 변환
        int index = level - 1;

        // 2. 인덱스가 0 이상이고, 리스트의 최대 크기보다 작은지(안전한지) 확인
        if (index >= 0 && index < LevelDatas.Count)
        {
            return LevelDatas[index].skillDescription;
        }

        return Description;
    }
    public void LevelUp()
    {
        skillLevel++;
        // 임시로 하드코딩
        skillLevel = Mathf.Clamp(skillLevel, 1, 3);
    }

    protected  int GetSkillLevel()
    {
        int index = Mathf.Min(skillLevel - 1, LevelDatas.Count - 1);
        int max = Mathf.Max(LevelDatas.Count - 1, 1);
        index = Mathf.Clamp(index, 0, max);

        return index;
    }
   
    public SkillLevelData GetCurrentLevelData()
    {
        if (LevelDatas.Count > 0 && LevelDatas.Count > skillLevel - 1)
            return LevelDatas[skillLevel - 1];

        return null;
    }

    public virtual void ApplyEffect() { }
}