

public enum MonsterGrade { NONE = 0, NORMAL = 1, ELITE, BOSS };

///////////////////////////////////////////////////////////////////////////////
//  Language
public enum LanguageType { KR, EN }


///////////////////////////////////////////////////////////////////////////////
//  StageType
[System.Serializable]
public enum StageType
{
    None = 0,
    Combat,
    Event,
    Shop,
    Boss_Combat,
}

///////////////////////////////////////////////////////////////////////////////
//  ItemCategory
[System.Serializable]
public enum ItemCategory
{
    NONE,
    CURRENCY = 1,       // 화폐
    EQUIPMENT = 2,      // 장비
    INGREDIANT,         // 재료 
    MAX,
}

///////////////////////////////////////////////////////////////////////////////
//  Currency
public enum CurrencyType
{
    NONE,
    GOLD,
    DIAMOND,
    EXPOLORE_COIN,
}

///////////////////////////////////////////////////////////////////////////////
//  Reward
  public enum RewardType
{
    NONE,
    ITEM,
    RECORD, 
    EXP,
}

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

///////////////////////////////////////////////////////////////////////////////
//  Equipment
public enum EquipParts
{
    NONE = -1,
    WEAPON = 0,
    CARFRAME = 1,
    CORE,
    ENGINE,
    SENSOR,
    WHEEL,
    MAX = 6,
}

///////////////////////////////////////////////////////////////////////////////
//  Item Rank 
public enum ItemRank
{
    NONE,
    COMMON,
    MAGIC,
    RARE,
    UNIQUE,
    LEGENDARY,

    MAX = LEGENDARY,
}

///////////////////////////////////////////////////////////////////////////////
//  Passive Trigger 
public enum TriggerEvent
{
    ON_GAME_START = 0,
    ON_ENTER_ROOM = 1,
    On_ATTACK,
    ON_DEAMGED,
    ON_ENENMY_KILLED
}




// 필터링을 위한 Enum (시트의 TargetFilter와 맞춤)
public enum TargetFilterType
{
    ALL,
    Shooter,
}

public enum RecordRarity
{
    NORMAL,
    RARE,
    UNIQUE,
    LEGENDARY,
    MYTH,
}

public enum RecordRewardMode
{
    RandomAll,
    RandomByRarity,
    FixedRecord
}


///////////////////////////////////////////////////////////////////////////////
// 이벤트 
public enum EventCostType
{
    NONE,

    CURRENCY,

    HP,

    RECORD,

    ITEM,
}

// 이벤트 보상
public enum EventRewardType
{
    NONE,

    HP_HEAL,
    HP_DAMAGE,

    ITEM,

    RECORD,

    BUFF,

    ARCHIVE_SAVE,
    ARCHIVE_LOAD

}

public enum EventActionType
{
    NONE,

    STAGE_COMBAT,

    RECORD_DRAFT,
    RECORD_UP,
    RECORD_SKILL_UP,

    ARCHIVE_SAVE,
    ARCHIVE_LOAD,

    APPLY_BUFF,
    APPLY_DEBUFF,

    OPEN_SHOP,
}

public enum EventCostParam
{
    NONE,

    GOLD,
    EXPLORE_COIN,

    FIXED,
    PERCENT,

    ANY,
    SAVED,
}
public enum EventActionParam
{
    NONE,

    // Buff
    MANA_REGEN_UP,
    ATTACK_UP,
    DEFENSE_UP,

    // Debuff
    MANA_REGEN_DOWN,
    ATTACK_DOWN,
    DEFENSE_DOWN,

    // Combat
    STAGE,

    // Shop
    RECORD_SHOP,
    CURSE_SHOP,
}

///////////////////////////////////////////////////////////////////////////////
//  Target Position Type 
public enum TargetPositionType
{
    CasterForward,      // 시전자의 정면 일정 거리
    FixedLocalOffset,   // 시전자 기준 특정 위치 (예: 내 오른쪽 2미터)
    RandomAroundCaster, // (미래용) 시전자 주변 무작위 위치 (메테오 샤워용)
    ReadFromBlackboard,  // (미래용) 마우스 클릭 등 외부에서 이미 세팅한 값 유지
    NearestEnemy,        // 가장 가까운 적 하나
    MultipleEnemies      // 범위 내 여러 명의 적
}

public enum FirePatternType
{
    RegularFan,   // 정해진 간격으로 쏘는 부채꼴 (기존 방식)
    RandomSpread  // 집탄율 범위 내에서 무작위 난사
}