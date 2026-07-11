using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

// 💡 Animator 요구 속성 제거! 오직 물리(Rigidbody2D)만 가집니다.
[RequireComponent(typeof(Rigidbody2D))]
public class Character
    : MonoBehaviour
    , ITeamAgent
{
    protected GenenricTeamId genericTeamId;

    protected new Rigidbody2D rigidbody;

    protected HealthPointComponent healthPoint;
    protected StatusComponent status;
    public StatusComponent Status => status;

    protected bool bInAction = false;
    public virtual bool InAction { get { return bInAction; } protected set { bInAction = value; } }

    // 슬로우 관리를 위한 토큰 (코루틴 대체)
    private CancellationTokenSource slowCts;

    #region ACTION
    public Action OnBeginDoAction;
    public Action OnEndDoAction;
    public Action<Character> OnDead;
    #endregion

    private int charID;
    public int CharID
    {
        get { return charID; }
        set { charID = value; }
    }

    protected virtual void Awake()
    {

        healthPoint = GetComponent<HealthPointComponent>();
        status = GetComponent<StatusComponent>();
        if (status != null && healthPoint != null)
            status.OnSetHealth += healthPoint.SetHealthPoint;
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnDisable()
    {
        OnDead = null;

        // 메모리 누수 방지
        if (slowCts != null)
        {
            slowCts.Cancel();
            slowCts.Dispose();
        }
    }

    public virtual void End_Damaged() { bInAction = false; }

    public void SetGenericTeamId(GenenricTeamId id) { genericTeamId = id; }
    public GenenricTeamId GetGeneriTeamId() { return genericTeamId; }

    #region AnimationEvent
    // 🚨 주의: 애니메이터가 자식(Model)으로 이동했으므로, Unity Animation Event는 Visual 스크립트를 때리게 됩니다.
    // Visual 스크립트에서 이 함수들을 호출해주도록 브릿지(Bridge) 연결이 필요합니다!
    public virtual void Start_DoAction() { }
    public virtual void Begin_DoAction() { OnBeginDoAction?.Invoke(); }
    public virtual void End_DoAction() { OnEndDoAction?.Invoke(); }
    public virtual void Begin_JudgeAttack(AnimationEvent e = null) { }
    public virtual void End_JudgeAttack(AnimationEvent e = null) { }
    public virtual void Play_Sound() { }
    public virtual void Play_CameraShake() { }
    #endregion

   
    public virtual void SetStatus() { }

    protected virtual void Dead() { }


    public static implicit operator GameObject(Character c)
    {
        return c != null ? c.gameObject : null;
    }
}