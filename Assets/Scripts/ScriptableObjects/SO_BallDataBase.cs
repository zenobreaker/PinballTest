using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BallData
{
    [Header("기본 정보")]
    public BallType ballType;
    public int level = 1; // 노멀볼은 1 고정, 나머지는 1~3
    public string poolName; // 예: "FireBall", "IceBall"
    public float speed = 15f;

    [Header("데미지 및 스탯")]
    public DamageData damageData; // 이곳의 baseDamage에 과제 명세의 21, 24, 25 등을 입력

    [Header("특수 효과 수치 (스킬별 선택 사용)")]
    public float effectChance; // 예: 아이스볼 30%, 35%...
    public float effectDuration; // 예: 화상 4초, 냉동 5초...
    public float effectValue; // 예: 화상 초당 8피해, 클러스터 10피해 특수볼 등
    public int maxStacks; // 예: 화상 최대 3중첩...
}

[CreateAssetMenu(fileName = "SO_BallDataBase", menuName = "Scriptable Objects/SO_BallDatabase")]
public class SO_BallDataBase : ScriptableObject
{
    [SerializeField] private List<BallData> ballDataList;

    // 런타임에 빠르게 조회하기 위한 캐시 딕셔너리
    // BallType과 Level을 묶어서 Key로 사용하는 딕셔너리
    private Dictionary<(BallType, int), BallData> ballDataDictionary;

    private void Initialize()
    {
        if (ballDataDictionary != null) return;

        ballDataDictionary = new Dictionary<(BallType, int), BallData>();

        foreach (var data in ballDataList)
        {
            var key = (data.ballType, data.level);
            if (!ballDataDictionary.ContainsKey(key))
            {
                ballDataDictionary.Add(key, data);
            }
        }
    }

    public BallData GetBallData(BallType type, int level)
    {
        Initialize(); // 최초 호출 시 딕셔너리 구성

        var key = (type, level);
        if (ballDataDictionary.TryGetValue(key, out BallData data))
        {
            return data;
        }

        Debug.LogError($"BallDatabase에 {type}의 Level {level} 데이터가 세팅되지 않았습니다.");
        return null;
    }
}
