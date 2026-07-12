using UnityEngine;

public static class DamageCalculator
{
    private readonly static float CONST_DEFNSE = 100.0f;
    private static int GLOBAL_ATTACK_ID_COUNTER = 0; 

    private static int GetNextAttackInstanceID()
    {
        if (GLOBAL_ATTACK_ID_COUNTER >= int.MaxValue)
            GLOBAL_ATTACK_ID_COUNTER = 0;
        return GLOBAL_ATTACK_ID_COUNTER++;
    }

    public static DamageEvent GetMyDamageEvent(StatusComponent status, DamageData data, 
        bool bFirstHit = false, bool bExtraCrit = false, float multiplier = 1.0f)
    {
        float attack = status.GetStatusValue(StatusType.ATTACK);
        float critRatio = status.GetStatusValue(StatusType.CRIT_RATIO);
        float critDmg = status.GetStatusValue(StatusType.CRIT_DMG);
        bool crit = false;

        float result = data.baseDamage + (attack * data.statCoefficient);
        result *= multiplier;

        DamageEvent evt = new DamageEvent(result, crit, bFirstHit, data.hitData);
        evt.AttackInstanceID = GetNextAttackInstanceID();
        evt.IgnoreDefenseRate = data.ignoreDefenseRate;

        evt.BaseCritChance = critRatio;
        evt.CritMultiplier = critDmg;

        return evt;
    }
    public static void EvaluateCrit(DamageEvent evt)
    {
        // 1. 강제 확정 크리티컬 (bExtraCrit) 처리
        if (evt.isCrit)
        {
            evt.BaseDamage *= evt.CritMultiplier;
            return;
        }

        // 2. 기본 확률 + 추가 확률(앞/뒤 타격 등) 합산
        float finalCritChance = evt.BaseCritChance + evt.ExtraCritChance;

        // 3. 주사위 굴리기!
        if (Random.Range(0.0f, 1.0f) <= finalCritChance)
        {
            evt.isCrit = true;
            evt.BaseDamage *= evt.CritMultiplier; // 크리티컬 성공 시 배율 적용
        }
    }


    public static float CalcDamage(StatusComponent status, DamageEvent damageEvent)
    {
        if (status == null || damageEvent == null)  
            return 0.0f;

        if (!damageEvent.isCrit)
        {
            EvaluateCrit(damageEvent);
        }

        float resultDamage = damageEvent.BaseDamage;
        float defense = status.GetStatusValue(StatusType.DEFENSE);
        float multiplier = 1.0f; // 데미지 증폭
        if( damageEvent.DamageAmp > 0.0f)
            multiplier *= damageEvent.DamageAmp;

        // 잃은 체력 비례 데미지
        if( damageEvent.MissingHPRatio > 0.0f)
        {
            float mx = status.GetMaxHP();
            float curhp = status.GetCurrentHP();
            float missingHP =  mx - curhp;
            resultDamage += missingHP * damageEvent.MissingHPRatio;
        }

        // 최대 체력 비례 데미지 
        if(damageEvent.MaxHPRatio > 0.0f)
        {
            resultDamage += status.GetMaxHP() * damageEvent.MaxHPRatio; 
        }

        // 방어 무시 
        if (damageEvent.IgnoreDefenseRate > 0f)
        {
            defense *= (1.0f - damageEvent.IgnoreDefenseRate);
        }

        //최소 방어력 제한
        // 💡 [비율 관통 적용] 방어력 무시 비율만큼 피격자의 방어력을 깎아버립니다. (0.0 ~1.0)
        defense = Mathf.Max(defense, -1.0f * CONST_DEFNSE + 0.001f);

        if (damageEvent.IgnoreDefenseRate >= 1.0f)
            return resultDamage * multiplier;

        // 데미지 계산 공식 
        // 피해 감소율 = 피격자 방어력 / (방어 상수 + 피격자 방어력)
        float ratio = defense / (CONST_DEFNSE + defense);

        return  Mathf.Round((resultDamage * (1 - ratio)) * multiplier);
    }
}
