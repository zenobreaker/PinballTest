using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
    : Singleton<GameManager>
{
    public enum GameState
    {
        NONE,
        BEGIN_STAGE,
        PROCESS_BATTLE,
        PROCESS_CHOICE,
        FINISH_STAGE,
    };

    private GameState state;

    public event Action OnBeginStage;
    public event Action OnBattleStage;
    public event Action OnFinishStage;
    public event Action<float> OnUpdated;

    private StageManager stageManager;
    public StageManager StageManager => stageManager;

    [SerializeField] private ResultPopup resultPopup;

    protected override void Awake()
    {
        base.Awake();

        if (IsDuplicate) return;

        stageManager = GetComponent<StageManager>();
    }

    private void OnEnable()
    {
        if(stageManager != null)
        {
            stageManager.OnProcessBattle += OnPrecessBattle;
        }
    }

    private void OnDisable()
    {
        if(stageManager != null)
        {
            stageManager.OnProcessBattle -= OnPrecessBattle;
        }
    }

    protected override void SyncDataFromSingleton()
    {
        base.SyncDataFromSingleton();
    }

    protected void Update()
    {
        if (state == GameState.PROCESS_BATTLE)
        {
            OnUpdated?.Invoke(Time.deltaTime);
        }
    }

    public void StartStage()
    {
        if (stageManager != null)
        {
            stageManager.InitializeStageData();
            stageManager.InitializeSpawnPoints();

            RunStageAsync().Forget();
        }
    }


    private async UniTaskVoid RunStageAsync()
    {
        SetGameState(GameState.BEGIN_STAGE);

        // 스테이지 돌리고 결과 가져오기
        StageResult result = await stageManager.RunStageFlowAsync(this.GetCancellationTokenOnDestroy());

        SetGameState(GameState.FINISH_STAGE);

        resultPopup.Show(result.IsSuccess);
    }

    public void OnPrecessBattle()
    {
        SetGameState(GameState.PROCESS_BATTLE);
    }

    private void SetGameState(GameState newState)
    {
        state = newState;
        switch (state)
        {
            case GameState.BEGIN_STAGE: OnBeginStage?.Invoke(); break;
            case GameState.PROCESS_BATTLE: OnBattleStage?.Invoke(); break;
            case GameState.FINISH_STAGE: OnFinishStage?.Invoke(); break;
        }
    }
}
