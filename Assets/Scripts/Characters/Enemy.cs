using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Enemy
    : Character
    , IDamagable
{
    [Header("Material Settings")]
    [SerializeField] private string[] surfaceNames;
    [SerializeField] private Color damageColor;
    [SerializeField] private float changeColorTime = 0.15f;

    private Color[] originColors;
    private Material[] skinMaterials;

    [SerializeField] private bool isBoss = false;
    public bool Boss { get => isBoss; set { isBoss = value; } }

    private DamageHandleComponent damageHandle;
    private MonsterGrade grade;
    private CancellationTokenSource damageCTS;

    protected override void Awake()
    {
        base.Awake();

        int index = 0;
        skinMaterials = new Material[surfaceNames.Length];
        originColors = new Color[surfaceNames.Length];
        foreach (string name in surfaceNames)
        {
            Transform surface = transform.FindChildByName(name);
            if (surface == null)
                continue;

            if (surface.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skin))
            {
                skinMaterials[index] = skin.material;
                originColors[index] = skin.material.color;
            }

            index++;
        }

        damageHandle = GetComponent<DamageHandleComponent>();
        Debug.Assert(damageHandle != null);
        damageHandle.OnDamagedEvent += HandleHitReaction;
    }

    protected override void Start()
    {
        base.Start();
        SetGenericTeamId(2);
    }

    protected void OnEnable()
    {
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        damageCTS?.Cancel();
        damageCTS?.Dispose();
        damageCTS = null;

        BattleManager.Instance.SafeInvoke(v => v.UnreistEnemy(this));
        CancelInvoke();
        ObjectPooler.ReturnToPool(gameObject);
    }

    public override void Start_DoAction()
    {
        base.Start_DoAction();
    }

    public override void End_DoAction()
    {
        base.End_DoAction();
        bInAction = false;

        OnEndDoAction?.Invoke();
    }

    public override void Begin_JudgeAttack(AnimationEvent e)
    {
        base.Begin_JudgeAttack(e);
    }

    public override void End_JudgeAttack(AnimationEvent e)
    {
        base.End_JudgeAttack(e);
    }

    public void OnDamage(GameObject attacker,
        Weapon causer, Vector3 hitPoint, DamageEvent damageEvent)
    {
        if (healthPoint != null && healthPoint.Dead)
            return;

        damageHandle.SafeInvoke(v => v.OnDamage(attacker, damageEvent));
        damageCTS?.Cancel();
        damageCTS?.Dispose();

        damageCTS = new CancellationTokenSource();

        Change_Color(0.15f, damageCTS.Token).Forget();

        if (healthPoint.Dead == false)
            return;

        HandleDeath().Forget();
    }


    private async UniTaskVoid Change_Color(float time, CancellationToken token)
    {
        try
        {
            foreach (Material mat in skinMaterials)
            {
                if (mat != null) mat.color = damageColor;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: token);

            int index = 0;
            foreach (Material mat in skinMaterials)
            {
                if (mat != null) mat.color = originColors[index];
                index++;
            }
        }
        catch (OperationCanceledException)
        {
            // 💡 몹이 죽거나 파괴되어 취소되었을 때 색깔 원상복구
            int index = 0;
            foreach (Material mat in skinMaterials)
            {
                if (mat != null) mat.color = originColors[index];
                index++;
            }
        }
    }

    public override void End_Damaged()
    {
        base.End_Damaged();

    }

    public void SetStatData(MonsterInfo info)
    {
        if (info == null)
            return;


        healthPoint.SafeInvoke(v => v.SetHealthPoint(info.hp));

        if (TryGetComponent<MovementComponent>(out var move))
            move.MoveSpeed = info.speed;

        status.SetStatusValue(StatusType.ATTACK, info.attack);
        status.SetStatusValue(StatusType.DEFENSE, info.defense);
    }

    private async UniTaskVoid HandleDeath()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        Dead();
    }

    protected override void Dead()
    {
        // 이미 파괴 절차에 들어간 객체라면 무시! (Missing 에러 방어)
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
            return;

        base.Dead();
        gameObject.SafeInvoke(v => v.SetActive(false));
    }


    private void HandleHitReaction(DamageEvent damgeEvent)
    {
        if (damgeEvent.IsDOTEffect()) return;

        if (isBoss)
        {

        }
        else
        {
         
        }
    }
}
