using UnityEngine;

// 스킬 사용 시 처리 이벤트 
public class SkillUseEvent
{
    public int SkillID;
    public string SkillName;
    public GameObject Owner;
}

public class PassiveSkill
    : Skill
{
    protected GameObject owner;

    public PassiveSkill() : base()
    {
    }
    public PassiveSkill(int skillID, string skillName, string skillDesc, Sprite skillIcon)
        : base(skillID, skillName, skillDesc, skillIcon)
    {

    }

    public PassiveSkill(SO_SkillData data)
        : base(data)
    {

    }
}

public class NormalBallDamagePassive : PassiveSkill
{
    public NormalBallDamagePassive(SO_PassiveSkillData data) : base(data)
    {
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        BallRuntimeData data =
            BallManager.Instance.GetBallRuntimeData();

        switch (SkillLevel)
        {
            case 1:
                data.NormalBallDamageAmp = 1.2f;
                break;
            case 2:
                data.NormalBallDamageAmp = 1.4f;
                break;
            case 3:
                data.NormalBallDamageAmp = 1.6f;
                break;
        }
    }
}

public class MagicMirrorPassive : PassiveSkill
{
    public MagicMirrorPassive(SO_PassiveSkillData data) : base(data)
    {
    }
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        BallRuntimeData data = BallManager.Instance.GetBallRuntimeData();

        // 레벨당 20%, 40%, 60% 증가값을 BounceBonusDamageAmp에 더해줍니다.
        switch (SkillLevel)
        {
            case 1: data.BounceBonusDamageAmp = 0.2f; break; // 기본 1.0 + 0.2
            case 2: data.BounceBonusDamageAmp = 0.4f; break;
            case 3: data.BounceBonusDamageAmp = 0.6f; break;
        }
    }
}

public class AmethystDaggerPassive : PassiveSkill
{

    public AmethystDaggerPassive(SO_PassiveSkillData data) : base(data)
    {
    }
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        BallRuntimeData data = BallManager.Instance.GetBallRuntimeData();

        // 치명타 확률 10%, 20%, 30% 증가
        switch (SkillLevel)
        {
            case 1: data.FrontHitCritChanceBonus = 0.1f; break;
            case 2: data.FrontHitCritChanceBonus = 0.2f; break;
            case 3: data.FrontHitCritChanceBonus = 0.3f; break;
        }
    }
}
public class EmeraldDaggerPassive : PassiveSkill
{
    public EmeraldDaggerPassive(SO_PassiveSkillData data) : base(data)
    {
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        BallRuntimeData data = BallManager.Instance.GetBallRuntimeData();

        // 치명타 확률 10%, 20%, 30% 증가
        switch (SkillLevel)
        {
            case 1: data.BackHitCritChanceBonus = 0.2f; break;
            case 2: data.BackHitCritChanceBonus = 0.3f; break;
            case 3: data.BackHitCritChanceBonus = 0.4f; break;
        }
    }
}

public class LastMatchPassive : PassiveSkill
{
    public LastMatchPassive(SO_PassiveSkillData data) : base(data)
    {
    }


    public override void ApplyEffect()
    {
        base.ApplyEffect();

        // 이전에 구독한 이벤트가 있다면 중복 방지를 위해 해제 후 재구독
        BattleManager.Instance.OnEnemyDied -= HandleEnemyDeath;
        BattleManager.Instance.OnEnemyDied += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Character enemy)
    {
        float explosionDamage = SkillLevel * 10f; // Lv.1: 10, Lv.2: 20, Lv.3: 30

        // 적 위치에 폭발 이펙트 스폰 및 주변 데미지 판정 (OverlapCircle 등 활용)
        Debug.Log($"[마지막 성냥] {enemy.name} 사망, 주변에 {explosionDamage} 피해 폭발!");

        // 1. 오브젝트 풀에서 폭발 프리팹 꺼내기 (프리팹 이름/태그 확인 필수!)
        GameObject boomObj = ObjectPooler.DeferredSpawnFromPool("BoomEffect", enemy.transform.position, Quaternion.identity);

        if (boomObj != null)
        {
            // 2. BoomEffect 컴포넌트를 찾아서 초기화 함수 실행
            if (boomObj.TryGetComponent<BoomEffect>(out var boom))
            {
                // 공격의 주체(오너)를 플레이어로 지정 (필요 시 BattleManager에서 가져옴)
                Character player = BattleManager.Instance.GetPrioritizedPlayer();

                // 데미지와 오너 정보를 넘기면서 폭발!
                boom.Init(explosionDamage, player);
            }
        }
        ObjectPooler.FinishSpawn(boomObj);
    }
}
