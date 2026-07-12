using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SkillLevelData
{
    public DamageData damageData;
    public List<int> bonusOptionList;
}
 

[CreateAssetMenu(fileName = "SO_SkillData", menuName = "Scriptable Objects/SO_SkillData")]
public class SO_SkillData : ScriptableObject
{
    [Header("Skill Info")]
    public int id;
    public string skillName;
    public string skillDescription;
    public int maxLevel;
    public Sprite skillImage;

    [Header("Skill Level ")]
    public List<SkillLevelData> levelDatas;

    public virtual Skill CreateSkill() { return null; }
}
