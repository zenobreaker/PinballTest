using UnityEngine;



public class ShootComponent : MonoBehaviour
{
    [Header("발사 설정")]
    [SerializeField] private SO_BallDataBase ballDatabase;
    [SerializeField] private Transform launchPoint;

    private Character owner; 

    private void Awake()
    {
        owner = GetComponent<Character>();

        if (BallManager.Instance == null)
            return;

        BallManager.Instance.OnLaunch += Shoot;
    }

    // BallManager가 코루틴을 돌며 호출하는 함수
    private void Shoot(BallRuntimeData runtimeData, Vector2 direction)
    {
        if (ballDatabase == null) return; 
        if (owner.IsDead) return; 

        // 데이터베이스에서 타입과 레벨에 맞는 최종 데이터를 가져옴
        BallData data = ballDatabase.GetBallData(runtimeData.BallType, runtimeData.Level);
        if (data == null) return;

        if (launchPoint == null) return; 

        // 1. 오브젝트 풀에서 공을 가져옵니다. 
        GameObject ballObj = ObjectPooler.DeferredSpawnFromPool(data.poolName, launchPoint);

        if (ballObj != null)
        {
            // 2. 공의 위치를 발사대로 초기화
            ballObj.transform.position = launchPoint.position;

            // 3. Ball 컴포넌트를 가져와서 발사 로직(초기화) 실행
            if (ballObj.TryGetComponent<Ball>(out var ball))
            {
                if (SkillManager.Instance != null)
                {
                    SkillLevelData skillData = SkillManager.Instance.
                        GetSkillDataByBallType(runtimeData.BallType);
                    ball.SetSkillData(skillData);
                }

                ball.SetUpData(owner, data, runtimeData);
                ObjectPooler.FinishSpawn(ballObj);
                ball.Launch(direction, data.speed);

            }
            else
            {
                Debug.LogWarning("발사된 오브젝트에 Ball 컴포넌트가 없습니다!");
            }
        }
    }
}