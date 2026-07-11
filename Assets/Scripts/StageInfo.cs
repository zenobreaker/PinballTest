using System.Collections.Generic;

[System.Serializable]
public class SpawnData
{
    public int monsterId;
    public int spawnPointIndex; // 에디터에서 배치한 MapGrid 안의 자식 인덱스
}

[System.Serializable]
public class WaveData
{
    public int waveIndex;
    public List<SpawnData> spawns = new List<SpawnData>();
}

[System.Serializable]
public class StageInfo
{
    public int id;
    public int wave = 0; // 총 웨이브 수

    // 기존의 groupIds 대신 WaveData 리스트를 갖습니다.
    public List<WaveData> waves = new List<WaveData>();

    public StageInfo() { }

    public StageInfo(StageInfo other)
    {
        id = other.id;
        wave = other.wave;
        waves = new List<WaveData>();

        foreach (var w in other.waves)
        {
            waves.Add(new WaveData
            {
                waveIndex = w.waveIndex,
                spawns = new List<SpawnData>(w.spawns)
            });
        }
    }

    public StageInfo Copy()
    {
        return new StageInfo(this);
    }
}