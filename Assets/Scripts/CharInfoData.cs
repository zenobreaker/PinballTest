using UnityEngine;

[System.Serializable]
public class StatGrowth
{
    public float baseValue;
    public float growth;
    public float bonus;
    public float exponent;

    public StatGrowth(float baseValue = 0.0f, float growth = 0.0f, float bonus = 0.0f, float exponent = 0.0f)
    {
        this.baseValue = baseValue;
        this.growth = growth;
        this.bonus = bonus;
        this.exponent = exponent;
    }

    public float GetValue(int level)
    {
        return baseValue
            + (level * growth)
            + (bonus * Mathf.Pow(level, exponent));
    }
}


[System.Serializable]
public class CharStatusData
{
    public int id;
    public int level;

    public StatGrowth hp;
    public StatGrowth attack;
    public StatGrowth defense;
    public StatGrowth attackSpeed;
    public StatGrowth moveSpeed;
    public StatGrowth critical;
    public StatGrowth critDamage;

    public virtual float GetStatusValue(StatusType type) { return 0.0f; }
}

