using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillLevelData
{
    [TextArea] public string skillDescription;

    [Header("기본 데미지 / 퍼센트 수치")]
    public float effectValue; // 데미지, 추가 피해량(%), 치명타 확률(%) 등

    [Header("상태이상 및 특수 효과")]
    public float effectChance;   // 발동 확률 (예: 빙결 30%, 클러스터 40%)
    public float effectDuration; // 지속 시간 (예: 화상 4초, 빙결 5초)
    public int maxStack;         // 최대 중첩 (예: 화상 3중첩)
}

[CreateAssetMenu(fileName = "SO_SkillData", menuName = "Scriptable Objects/SO_SkillData")]
public class SO_SkillData : ScriptableObject
{
    [Header("Skill Info")]
    public int id;
    public string skillName;

    public int maxLevel = 3;
    public Sprite skillImage;

    [Header("Skill Level ")]
    public List<SkillLevelData> levelDatas;

    public virtual Skill CreateSkill() { return null; }
}
