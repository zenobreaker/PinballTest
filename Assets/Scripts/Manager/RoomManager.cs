using System.Collections.Generic;
using UnityEngine;
public class RoomData
{
    public List<Transform> MainSpawnPoints = new List<Transform>();
    public List<Transform> EnemySpawnPoints = new List<Transform>();
}

public sealed class RoomManager : MonoBehaviour
{
    private BiomeManager biomeManager;

    private void Awake()
    {
        biomeManager = GetComponent<BiomeManager>();
    }

    public RoomData LoadRoom(StageInfo curStage)
    {
        RoomData roomData = new RoomData();

        if (curStage == null)
            return roomData;

        //var mainSpawnRoot = map.transform.Find("MainSpawnPoints");
        //if (mainSpawnRoot != null)
        //{
        //    foreach (Transform t in mainSpawnRoot)
        //        roomData.MainSpawnPoints.Add(t);
        //}

        //var spawnRoot = map.transform.Find("SpawnPoints");
        //if (spawnRoot != null)
        //{
        //    foreach (Transform t in spawnRoot)
        //        roomData.EnemySpawnPoints.Add(t);
        //}

        return roomData;
    }
}
