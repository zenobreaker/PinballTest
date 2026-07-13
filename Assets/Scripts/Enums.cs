

public enum MonsterGrade { NONE = 0, NORMAL = 1, ELITE, BOSS };

///////////////////////////////////////////////////////////////////////////////
//  Status
public enum StatusType
{
    NONE = 0,
    ATTACK = 1,
    DEFENSE = 2,
    ATTACKSPEED = 3,
    MOVESPEED = 4,
    CRIT_RATIO = 5,
    CRIT_DMG = 6,

    HEALTH,
    HEALTH_REGEN,

    MAX,
}

///////////////////////////////////////////////////////////////////////////////
//  Damage 
public enum DamageType
{
    NORMAL = 0,
    DOT_BLEED,
    DOT_BURN,
    DOT_POISON,

    MAX
}




///////////////////////////////////////////////////////////////////////////////
//  Buff
public enum BuffStackPolicy
{
    REFRESH_ONLY = 0,
    STACKABLE,
    IGNOREIFEXSIST,
}

public enum ModifierValueType
{
    FIXED,
    PERCENT,
}

///////////////////////////////////////////////////////////////////////////////
//  Effect Type 
public enum EffectType
{
    NONE,
    BUFF,
    DEBUFF,
    NEUTRAL,
}

