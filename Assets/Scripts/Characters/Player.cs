using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Player
    : Character
    , IDamagable
{
    private DamageHandleComponent damageHandle;

    private int jobID;
    public int JobID
    {
        get { return jobID; }
        set { jobID = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        damageHandle = GetComponent<DamageHandleComponent>();
    }



    protected override void Start()
    {
        base.Start();

        SetGenericTeamId(1);

        BattleManager.Instance.SafeInvoke(v => v.RegistPlayer(this));
        BallManager.Instance.SafeInvoke(v => v.Init(this));
        GameManager.Instance.SafeInvoke(v => v.StartStage());
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        BattleManager.Instance.SafeInvoke(v => v.UnreistPlayer(this));
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        OnBeginDoAction?.Invoke();
    }

    public override void End_DoAction()
    {
        Debug.Log("Player End DoAction");

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

    public override void Play_Sound()
    {
        base.Play_Sound();
    }

    public override void Play_CameraShake()
    {
        base.Play_CameraShake();
    }

    public void OnDamage(GameObject attacker, Ball causer, Vector3 hitPoint, DamageEvent damageEvent)
    {

        // 이 함수 내부에서 이미 HP를 깎고 state.SetDamagedMode()를 호출
        damageHandle.SafeInvoke(v => v.OnDamage(attacker, damageEvent));

        Debug.Log($"{healthPoint.GetCurrentHP} hp 남음");

        // 살았는지 죽었는지 판단
        if (healthPoint.Dead == false)
        {
            return;
        }
        // --- 여기서부터는 죽었을 때의 처리 ---

        // 코루틴 대신 UniTask 호출
        HandleDeath().Forget();
    }

    // 💡 IEnumerator -> async UniTaskVoid 로 변경
    private async UniTaskVoid HandleDeath()
    {
        // 1초 대기 (토큰이 없으므로 씬 전환 시 에러 안 나게 주의)
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        Dead();
    }

    protected override void Dead()
    {
        base.Dead();
        Destroy(gameObject);
    }
}
