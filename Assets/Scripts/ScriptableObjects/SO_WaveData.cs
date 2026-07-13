// 1. 웨이브 데이터 SO
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "Scriptable Objects/WaveData")]
public class SO_WaveData : ScriptableObject
{
    public int waveIndex;
    public List<SpawnData> spawns = new List<SpawnData>();
}