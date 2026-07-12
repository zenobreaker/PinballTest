using System;
using System.Collections.Generic;
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

    }

    public Skill(int id, string name, string desc, Sprite icon)
    {
        skillID = id;
        skillName = name;
        skillDescription = desc;
        Icon = icon;
    }

    public Skill(SO_SkillData skillData)
        : this(skillData.id, skillData.skillName, skillData.skillDescription, skillData.skillImage)
    {
        LevelDatas = skillData.levelDatas;
    }

    public virtual void SetLevel(int level)
    {
        skillLevel = level;
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