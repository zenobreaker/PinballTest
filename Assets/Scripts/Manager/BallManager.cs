using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


[Serializable]
public class BallRuntimeData
{
    public BallType BallType;

    public int Level = 1;
}

public class BallManager : Singleton<BallManager>
{
    [Header("발사 설정")]
    public int maxBallCount = 4;
    [SerializeField] private float launchInterval = 0.15f; // 공 발사 간격
    [SerializeField] private float autoFireDelay = 1.0f; // 공이 모두 회수된 후 다음 발사까지의 대기 시간

    // 현재 보유 중인 볼 런타임 데이터 리스트
    private List<BallRuntimeData> currentBalls = new List<BallRuntimeData>();

    private int activeBallCount;
    private int currentBallIndex = 0;
    private int currentShootIndex = 0;

    // 발사 루틴 및 예약 제어용 변수
    private bool isRoutineRunning = false;
    // 자동 발사를 위한 타겟 방향 (초기값: 90도 위쪽)
    private Vector2 currentAimDirection = Vector2.up;


    public event Action<BallRuntimeData, Vector2> OnLaunch;

    private Character cachedPlayer;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(Character player )
    {
        activeBallCount = 0;
        currentBallIndex = 0;
        currentShootIndex = 0; 
        isRoutineRunning = false;
        currentAimDirection = Vector2.up; // 초기 방향 90도 고정

        if (player == null)
            cachedPlayer = BattleManager.Instance.GetPrioritizedPlayer();
        else
            cachedPlayer = player;

            currentBalls.Clear();
        for (int i = 0; i < maxBallCount; i++)
        {
            currentBalls.Add(new BallRuntimeData { BallType = BallType.Normal, Level = 1 });
        }

        ScheduleNextTurn().Forget();
    }

    public bool GetAvailableBallPool()
    {
        bool isAvailable = true;

        int count = 0; 
        foreach (var ball in currentBalls)
        {
            if(ball.Level >= 3 || ball.BallType != BallType.Normal)
            {
                count++;
            }
        }

        if (count >= 4)
            isAvailable = false;

        return isAvailable; 
    }

    // 삼택지에서 새로운 볼 획득 또는 업그레이드 시 호출할 함수

    public void ReplaceOrUpgradeBall(ActiveSkill ballSkill)
    {
        if (ballSkill == null) return;

        ReplaceOrUpgradeBall(ballSkill.BallType, ballSkill.SkillLevel);
    }

    public void ReplaceOrUpgradeBall(BallType newType, int level)
    {
        int existingIndex = currentBalls.FindIndex(b => b.BallType == newType);

        if (existingIndex != -1)
        {
            // [업그레이드] 이미 존재하면 레벨만 갱신
            currentBalls[existingIndex].Level = level;
            Debug.Log($"[BallManager] {newType} 볼 레벨 업: {level}");
        }
        else
        {
            // [새로운 볼 획득] 비어있는 슬롯(혹은 다음 슬롯)에 추가
            // 만약 현재 4개 미만이라면 추가하고, 4개라면 기존 로직에 따라 덮어쓰거나 처리
            if (currentBalls.Count < maxBallCount)
            {
                currentBalls.Add(new BallRuntimeData { BallType = newType, Level = level });
                Debug.Log($"[BallManager] 새로운 볼 {newType} 추가 (Level: {level})");
            }
            else
            {
                // 이미 4개가 꽉 찼는데 새로운 타입이라면, 기존 정책(예: 인덱스 순차 교체)대로 처리
                // 여기서는 예시로 첫 번째 슬롯부터 교체하도록 구현했습니다.
                currentBalls[currentBallIndex % maxBallCount].BallType = newType;
                currentBalls[currentBallIndex % maxBallCount].Level = level;
                currentBallIndex++;
            }
        }
    }

    public void SetAimDirection(Vector2 direction)
    {
        currentAimDirection = direction.normalized;
    }

    // 다음 턴을 준비하고 자동 발사하는 비동기 함수
    private async UniTaskVoid ScheduleNextTurn()
    {
        // 지정된 시간(interval)만큼 대기
        await UniTask.Delay(TimeSpan.FromSeconds(autoFireDelay), cancellationToken: this.GetCancellationTokenOnDestroy());

        TryShoot();
    }

    private void TryShoot()
    {
        if (cachedPlayer != null && cachedPlayer.IsDead)
            return;

        // 발사 루틴이 이미 돌고 있거나, 공을 다 썼다면 종료
        if (isRoutineRunning) return;

        // 쏠 공이 하나라도 남아있다면 발사 루틴 시작
        if (activeBallCount < maxBallCount)
        {
            LaunchRoutineAsync(currentAimDirection, this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    private async UniTaskVoid LaunchRoutineAsync(Vector2 direction, CancellationToken cancellationToken)
    {
        isRoutineRunning = true;

        // 쏠 수 있는 공이 남아있을 때까지 반복 (이 루틴은 처음에 받은 direction을 끝까지 유지함)
        if (activeBallCount < maxBallCount)
        {
            BallRuntimeData ballToLaunch = currentBalls[currentShootIndex];
            OnLaunch?.Invoke(ballToLaunch, direction);
            activeBallCount++;
            currentShootIndex = (currentShootIndex + 1) % maxBallCount;
        }

        // 지정된 발사 간격만큼 대기 후 루틴 종료
        await UniTask.Delay(TimeSpan.FromSeconds(launchInterval), cancellationToken: cancellationToken);

        // 루틴이 끝남
        isRoutineRunning = false;

        TryShoot();
    }

    // 공이 바닥에 닿아 회수될 때 호출
    public void OnBallReturned()
    {
        activeBallCount--;

        // 공이 돌아왔으니 쏠 수 있는 여유가 생겼음 -> 즉시 발사 시도
        TryShoot();
    }
}