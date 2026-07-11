using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterInfo
{
    [Header("기본 식별 정보")]
    public int id;
    public string monsterName;
    public string poolName; // ObjectPooler에서 사용할 프리팹 태그

    [Header("전투 스탯")]
    public int hp;
    public int attack;
    public int defense;
    public float speed;

    [Header("그리드 설정")]
    [Tooltip("타일 크기 (x: 가로, y: 세로). 예: 1x1은 (1, 1), 가로 2칸은 (2, 1)")]
    public Vector2Int tileScale = new Vector2Int(1, 1);
}

[CreateAssetMenu(fileName = "SO_MonsterDatabase", menuName = "Scriptable Objects/MonsterDatabase")]
public class SO_MonsterDatabase : ScriptableObject
{
    [SerializeField]
    private List<MonsterInfo> monsterList = new List<MonsterInfo>();

    // 런타임 빠른 검색을 위한 캐시 딕셔너리
    private Dictionary<int, MonsterInfo> monsterDict;

    private void Initialize()
    {
        // 이미 딕셔너리가 초기화되어 있다면 패스
        if (monsterDict != null) return;

        monsterDict = new Dictionary<int, MonsterInfo>();

        foreach (var monster in monsterList)
        {
            // ID 중복 방지 방어 코드
            if (!monsterDict.ContainsKey(monster.id))
            {
                monsterDict.Add(monster.id, monster);
            }
            else
            {
                Debug.LogWarning($"[SO_MonsterDatabase] 중복된 몬스터 ID가 발견되었습니다: {monster.id}");
            }
        }
    }

    /// <summary>
    /// ID를 통해 몬스터 정보를 반환합니다.
    /// </summary>
    public MonsterInfo GetMonsterData(int id)
    {
        Initialize(); // 최초 호출 시 한 번만 딕셔너리 구성됨

        if (monsterDict.TryGetValue(id, out MonsterInfo data))
        {
            return data;
        }

        Debug.LogError($"[SO_MonsterDatabase] ID {id}에 해당하는 몬스터 데이터가 존재하지 않습니다.");
        return null;
    }
}