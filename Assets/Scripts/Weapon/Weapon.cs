using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class HitData
{
    public DamageType DamageType;
    public float Distance;
    public float HeightValue;
    public int StopFrame;
    public int HitImpactIndex;
    public string HitSoundName;
    public GameObject HitParticle;
    public Vector3 HitParticlePositionOffset = Vector3.zero;
    public Vector3 HitParticleScaleOffset = Vector3.one;

    public void PlayHitSound()
    {
        //SoundManager.Instance.PlaySFX(HitSoundName);
    }

    public bool IsDOTEffect()
    {
        return DamageType == DamageType.DOT_BLEED || 
            DamageType == DamageType.DOT_BURN || 
            DamageType == DamageType.DOT_POISON;
    }

    public HitData Clone()
    {
        HitData clone = new HitData();
        clone.DamageType = DamageType;
        clone.Distance = Distance;
        clone.HeightValue = HeightValue;
        clone.StopFrame = StopFrame;
        clone.HitImpactIndex = HitImpactIndex;
        clone.HitSoundName = HitSoundName;
        clone.HitParticle = HitParticle;
        clone.HitParticlePositionOffset = HitParticlePositionOffset;
        clone.HitParticleScaleOffset = HitParticleScaleOffset;
        return clone;
    }
}


[Serializable]
public class DamageData
{
    [Header("Power Settings")]
    public DamageType damageType;

    [Tooltip("스킬 고유의 기본 데미지 (깡뎀)")]
    public float baseDamage = 10.0f;

    [Tooltip("캐릭터 스탯 반영 비율 (1.0 = 공격력의 100%)")]
    public float statCoefficient = 0.0f;

    [Header("Sound")]
    public string SoundName;

    [Header("Hit")]
    public HitData hitData;

    public float ignoreDefenseRate = 0.0f;

    public DamageEvent GetMyDamageEvent(
        StatusComponent attackerStatus,
        bool isFirstHit = false,
        bool extraCrit = false,
        float multiplier = 1f)
    {
        return DamageCalculator.GetMyDamageEvent(attackerStatus, this,isFirstHit,extraCrit, multiplier);
    }

    public void PlayHitSound()
    {
        if (hitData == null) return;

        hitData.PlayHitSound();
    }

    public DamageData Clone()
    {
        DamageData clone = new DamageData();
        clone.damageType = this.damageType;
        clone.baseDamage = this.baseDamage;
        clone.statCoefficient = this.statCoefficient;
        clone.SoundName = SoundName;
        if(hitData != null)
            clone.hitData = this.hitData.Clone();
        clone.ignoreDefenseRate = this.ignoreDefenseRate;
        return clone;
    }
}


[Serializable]
public class ActionData
{
    [SerializeField]
    private string subStateName;
    public string SubStateName { get => subStateName; }

    [SerializeField]
    private string stateName;
    private string state;
    public string StateName { get => state; }

    [SerializeField]
    private string layerName;
    public string LayerName { get => layerName; }

    [SerializeField] private float actionSpeed = 1.0f;
    public float ActionSpeed { get => actionSpeed; set => actionSpeed = value; }


    // StateName을 해시 값으로 저장
    private int actionSpeedHash = -1;
    public int ActionSpeedHash
    {
        get
        {
            if (actionSpeedHash == -1)
                actionSpeedHash = Animator.StringToHash("ActionSpeed");
            return actionSpeedHash;
        }
    }

    [SerializeField]
    private string weaponActionName;
    public string WeaponActionName { get => weaponActionName; }

    [Header("Sound")]
    public string SoundName;

    [Header("Camera Shake")]
    public Vector3 impulseDirection;

    //public SO_CameraShakePreset csp;

    [Header("ETC")]
    public bool bCanMove;
    //public bool bFixedCamera;

    public virtual ActionData Clone()
    {
        ActionData actionData = new ActionData();
        Initialize();
        actionData = (ActionData)MemberwiseClone();
       // actionData.csp = csp;
        return actionData;
    }

    public void Initialize()
    {
        if (string.IsNullOrEmpty(SubStateName) == false)
        {
            state = stateName + '.' + SubStateName;
            return;
        }
        state = stateName;
    }

    public void Play_Sound()
    {
       // SoundManager.Instance.PlaySFX(SoundName);
    }

    public void Play_CameraShake()
    {
        //if (MovableCameraShaker.Instance != null && csp != null)
        //    MovableCameraShaker.Instance.Play_Impulse(csp.settings);
    }
}


public class DamageEvent
{
    public float BaseDamage;
    public bool isCrit;
    public bool isFisrtHit;

    public HitData hitData;

    public float MissingHPRatio; // 잃은 체력 비례 데미지
    public float IgnoreDefenseRate;  // 방어력 무시 데미지
    public float MaxHPRatio; // 최대 체력 비례 데미지 
    public float DamageAmp;
    public float ExtraCritChance;
    public float BaseCritChance;
    public float CritMultiplier;

    public int AttackInstanceID { get; set; }
    public DamageEvent(float value, bool isCrit = false, bool isFisrtHit = false, HitData hitData = null)
    {
        this.BaseDamage = value;
        this.isCrit = isCrit;
        this.isFisrtHit = isFisrtHit;
        MissingHPRatio = 0f;
        IgnoreDefenseRate = 0f;
        MaxHPRatio = 0f;
        DamageAmp = 0f;

        if (hitData != null)
            this.hitData = hitData;
        else
            this.hitData = new();
    }

    public void AddCritChane(float value)
    {
        ExtraCritChance += value;
    }
    public bool IsDOTEffect()
    {
        if (hitData == null) return false;
        return hitData.IsDOTEffect();
    }

    public DamageEvent Clone()
    {
        // 1. 생성자를 이용해 기본 값 초기화 (hitData는 참조 복사)
        DamageEvent cloneEvent = new DamageEvent(
            this.BaseDamage,
            this.isCrit,
            this.isFisrtHit,
            this.hitData
        );

        // 2. 추가적인 각종 비율 및 배율 데이터 복사
        cloneEvent.MissingHPRatio = this.MissingHPRatio;
        cloneEvent.IgnoreDefenseRate = this.IgnoreDefenseRate;
        cloneEvent.MaxHPRatio = this.MaxHPRatio;
        cloneEvent.DamageAmp = this.DamageAmp;
        cloneEvent.ExtraCritChance = this.ExtraCritChance;
        cloneEvent.BaseCritChance = this.BaseCritChance; 
        cloneEvent.CritMultiplier = this.CritMultiplier; 
        cloneEvent.AttackInstanceID = this.AttackInstanceID;
        cloneEvent.hitData = this.hitData.Clone();

        return cloneEvent;
    }
}


