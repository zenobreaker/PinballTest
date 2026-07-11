using UnityEngine;
using System.Collections.Generic;

// 1. 웨이브 데이터 SO
[CreateAssetMenu(fileName = "NewWaveData", menuName = "Scriptable Objects/WaveData")]
public class SO_WaveData : ScriptableObject
{
    public int waveIndex;
    public List<SpawnData> spawns = new List<SpawnData>();
}

// 2. 스테이지 정보 SO
[CreateAssetMenu(fileName = "NewStageInfo", menuName = "Scriptable Objects/StageInfo")]
public class SO_StageInfo : ScriptableObject
{
    public int stageId;
    [Tooltip("이 스테이지에 포함될 웨이브 SO들을 순서대로 넣어주세요.")]
    public List<SO_WaveData> waves = new List<SO_WaveData>();

    // StageManager에서 이 SO를 받아 런타임용 클래스로 변환할 때 사용합니다.
    public StageInfo ToRuntimeData()
    {
        StageInfo info = new StageInfo();
        info.id = this.stageId;
        info.wave = waves.Count; // 리스트의 개수가 곧 총 웨이브 수
        info.waves = new List<WaveData>();

        foreach (var waveSO in waves)
        {
            if (waveSO == null) continue;

            WaveData waveData = new WaveData();
            waveData.waveIndex = waveSO.waveIndex;
            waveData.spawns = new List<SpawnData>();

            // 스폰 데이터 복사
            foreach (var spawnSO in waveSO.spawns)
            {
                waveData.spawns.Add(new SpawnData
                {
                    monsterId = spawnSO.monsterId,
                    spawnPointIndex = spawnSO.spawnPointIndex
                });
            }
            info.waves.Add(waveData);
        }

        return info;
    }
}