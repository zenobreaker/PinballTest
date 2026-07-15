using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct StageResult
{
    public bool IsSuccess;
    public bool IsPlayerDead;
    public bool IsExploreFinished;
    public int ClearedWave;
}


public sealed class StageManager : MonoBehaviour
{
    public enum StageState
    {
        None,
        Preparing,
        Battle,
        Choice,
        Result
    };

    public StageState stageState;

    [Header("스테이지 데이터 설정")]
    [Tooltip("에디터에서 만든 SO_StageInfo를 넣어주세요.")]
    [SerializeField] private SO_StageInfo soStageInfo;
    [SerializeField] private WaveGroup waveGroup;


    [Header("스폰 설정")]
    [Tooltip("에디터에서 생성한 MapGrid 오브젝트를 여기에 드래그 앤 드롭 하세요.")]
    [SerializeField] private Transform spawnPointContainer;

    [SerializeField] private SkillSelectionUI selectionUI;

    private StageInfo currentStage;

    private SpawnManager spawnManager;

    // 수집된 스폰 포인트들을 담을 리스트
    private List<Transform> enemySpawnPoints = new List<Transform>();

    private bool bEnableSpawn = false;
    private bool needSkillChoice = false;

    public event Action OnProcessBattle;

    private void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }


    public void SetWaveGroup(WaveGroup waveGroup)
    {
        this.waveGroup = waveGroup;
    }

    public void SetSkillSelectionUI(SkillSelectionUI ui)
    {
        selectionUI = ui; 
    }

    public void InitializeStageData()
    {
        if (soStageInfo != null)
        {
            // SO 데이터를 런타임 클래스(StageInfo)로 반환받아 덮어씌움
            currentStage = soStageInfo.ToRuntimeData();
            Debug.Log($"[StageManager] 스테이지 {currentStage.id} 데이터를 성공적으로 로드했습니다. 총 웨이브: {currentStage.wave}");
        }
        else
        {
            Debug.LogError("[StageManager] SO_StageInfo가 인스펙터에 연결되지 않았습니다!");
        }
    }

    // 자식 오브젝트(SpawnPoint)들을 수집하는 함수
    public void InitializeSpawnPoints()
    {
        enemySpawnPoints.Clear();

        if (spawnPointContainer == null)
            spawnPointContainer = GameObject.FindFirstObjectByType<GridBuilder>()?.transform;

        if (spawnPointContainer != null)
        {
            foreach (Transform child in spawnPointContainer)
            {
                enemySpawnPoints.Add(child);
            }
            Debug.Log($"[StageManager] 총 {enemySpawnPoints.Count}개의 스폰 포인트를 로드했습니다.");
        }
        else
        {
            Debug.LogWarning("[StageManager] Spawn Point Container(MapGrid)가 인스펙터에 할당되지 않았습니다!");
        }
    }

    private void OnEnable()
    {
        ObjectPooler.OnPoolInitialized += OnPoolReady;
        if (ExperienceManager.Instance != null)
            ExperienceManager.Instance.OnLevelUp += HandleLevelUP;
    }

    private void OnDisable()
    {
        ObjectPooler.OnPoolInitialized -= OnPoolReady;
        if (ExperienceManager.Instance != null)
            ExperienceManager.Instance.OnLevelUp -= HandleLevelUP;
    }

    private void OnPoolReady()
    {
        Debug.Log("[StageManager] Pool Ready! 스테이지 생성을 시작합니다.");
        bEnableSpawn = true;
    }

    public void ResetStageData()
    {
        bEnableSpawn = false;
        stageState = StageState.None;
    }

    /// <summary>
    /// 외부(ExploreManager)에서 이 함수를 await로 호출하여 스테이지를 진행합니다.
    /// </summary>
    public async UniTask<StageResult> RunStageFlowAsync(CancellationToken token)
    {
        try
        {
            if (currentStage == null)
                return new StageResult();

            stageState = StageState.Preparing;
            int currentWave = 1;

            // 풀러 대기
            while (!bEnableSpawn)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }

            Character player = BattleManager.Instance.SafeInvoke(v => v.GetPrioritizedPlayer());
            bool isPlayerDead = false;
            if (player != null)
                player.OnDead += (player) => { isPlayerDead = true; };

            needSkillChoice = false;
            spawnManager.ClearEnemies(); 
            // 웨이브 루프 시작!
            while (currentWave <= currentStage.wave)
            {
                Debug.Log($"현재 웨이브 : {currentWave} ");
                waveGroup.SafeInvoke(v => v.DrawText(currentWave));

                // 적 스폰 대기
                await spawnManager.SpawnEnemiesAsync(currentStage.waves[currentWave - 1],
                    enemySpawnPoints, token);

                stageState = StageState.Battle;
                OnProcessBattle?.Invoke();

                //  전투 끝날 때까지 여기서 무한 대기! (이벤트 체인 불필요)
                while (!isPlayerDead && spawnManager.ActiveEnemyCount > 0)
                {
                    if (needSkillChoice)
                    {
                        needSkillChoice = false;

                        stageState = StageState.Choice;
                        
                        Time.timeScale = 0;

                        await selectionUI.SafeInvoke(v => v.ShowAsync());
                        
                        Time.timeScale = 1;

                        stageState = StageState.Battle;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                if (isPlayerDead)
                {
                    break; // 사망 시 즉시 루프 탈출
                }


                currentWave++;
            }

            stageState = StageState.Result;

            // 결과 포장해서 던지기
            return new StageResult
            {
                IsSuccess = !isPlayerDead,
                IsPlayerDead = isPlayerDead,
                ClearedWave = currentWave - 1
            };
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[StageManager] 스테이지 진행 취소됨");
            return new StageResult { IsSuccess = false, IsPlayerDead = true, ClearedWave = 0 };
        }
    }

    public void SetEnteredStage(StageInfo stage)
    {
        if (stage == null) return;

        currentStage = stage;
    }

    private void HandleLevelUP()
    {
        needSkillChoice = true;
    }

}
