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
        string result = "";
        if (LevelDatas.Count > 0 && LevelDatas.Count <= level - 1)
            result = LevelDatas[level - 1].skillDescription;
        return result;
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

    public virtual void ApplyEffect() { }
}