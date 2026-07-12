using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public sealed class SpawnManager : MonoBehaviour
{
     [SerializeField] private SO_MonsterDatabase monsterDatabase; // 몬스터 데이터베이스 연결용 (가정)

    private List<Character> spawnedEnemies = new();

    public event Action OnAllEnemiesDead;

    // 💡 StageManager가 대기할 때 사용할 프로퍼티 추가
    public int ActiveEnemyCount => spawnedEnemies.Count;


    // 💡 풀러 콜백을 기다려주는 진짜 비동기 NPC 스폰 함수!
    public async UniTask SpawnEnemiesAsync(WaveData waveData, List<Transform> spawnPoints, CancellationToken token)
    {
        if (waveData == null || waveData.spawns.Count <= 0) return;
        if (spawnPoints == null || spawnPoints.Count <= 0) return;

        var tcs = new UniTaskCompletionSource();
        int totalToSpawn = waveData.spawns.Count;
        int spawnedCount = 0;

        foreach (var spawnData in waveData.spawns)
        {
            // 인덱스 초과 방지 (방어 코드)
            if (spawnData.spawnPointIndex < 0 || spawnData.spawnPointIndex >= spawnPoints.Count)
            {
                Debug.LogWarning($"[SpawnManager] 스폰 인덱스 오류: {spawnData.spawnPointIndex}. 해당 몬스터는 스폰을 건너뜁니다.");
                spawnedCount++;
                if (spawnedCount >= totalToSpawn) tcs.TrySetResult();
                continue;
            }

            Transform targetPoint = spawnPoints[spawnData.spawnPointIndex];

            // 1. MonsterDatabase에서 id를 기반으로 풀링 태그(이름)를 가져온다고 가정
             string poolTag = monsterDatabase.GetMonsterData(spawnData.monsterId).poolName;

            // 2. 콜백을 포함한 풀링 스폰 처리
            ObjectPooler.DeferredSpawnWithCallback(poolTag, targetPoint, (npc) =>
            {
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                if (enemyLayer != -1)
                    SetLayerRecursively(npc, enemyLayer);

                if (npc.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    spawnedEnemies.Add(enemy);
                    enemy.OnDead += OnEnemyDead; // 적 사망 이벤트 구독
                    if (BattleManager.Instance != null)
                        enemy.OnDead += BattleManager.Instance.NotifyEnemyDie;

                    enemy.SetStatData(monsterDatabase.GetMonsterData(spawnData.monsterId));
                }

                ObjectPooler.FinishSpawn(npc);

                spawnedCount++;
                if (spawnedCount >= totalToSpawn)
                {
                    tcs.TrySetResult(); // 현재 웨이브 몬스터 전원 스폰 완료!
                }
            });
        }

        await tcs.Task; // 콜백이 모두 돌아올 때까지 대기
    }


    private void OnEnemyDead(Character enemy)
    {
        spawnedEnemies.Remove(enemy);
        
        ExperienceManager.Instance.SafeInvoke(v => v.AddExp(1));

        if (spawnedEnemies.Count == 0)
            OnAllEnemiesDead?.Invoke(); 
    }

    // 자식 오브젝트들까지 모조리 레이어를 바꿔주는 마법의 헬퍼 함수
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
