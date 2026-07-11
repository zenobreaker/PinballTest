using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class WaveDataGeneratorWindow : EditorWindow
{
    private GridBuilder gridBuilder;
    private int waveIndex = 1;
    private int selectedMonsterId = 1001;
    private Dictionary<int, int> gridData = new Dictionary<int, int>();

    [MenuItem("Tools/Wave Data Generator (웨이브 맵 메이커)")]
    public static void ShowWindow()
    {
        // 가로 사이즈를 충분히 확보하여 창을 엽니다.
        GetWindow<WaveDataGeneratorWindow>("Wave Generator").minSize = new Vector2(800, 500);
    }

    private void OnGUI()
    {
        GUILayout.Label("웨이브 스폰 데이터 생성기", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 전체를 가로로 배치 (왼쪽: 컨트롤 영역, 오른쪽: 맵 영역)
        EditorGUILayout.BeginHorizontal();

        // --- 왼쪽 영역: 컨트롤 및 버튼 ---
        EditorGUILayout.BeginVertical(GUILayout.Width(250));

        gridBuilder = (GridBuilder)EditorGUILayout.ObjectField("Grid Builder", gridBuilder, typeof(GridBuilder), true);
        EditorGUILayout.Space(10);

        waveIndex = EditorGUILayout.IntField("Wave Index", waveIndex);
        selectedMonsterId = EditorGUILayout.IntField("Monster ID", selectedMonsterId);

        EditorGUILayout.Space(20);

        // 버튼들을 왼쪽에 모아서 배치
        if (GUILayout.Button("SO_WaveData 저장", GUILayout.Height(40)))
        {
            SaveToScriptableObject();
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("그리드 초기화", GUILayout.Height(30)))
        {
            gridData.Clear();
        }

        EditorGUILayout.EndVertical();

        // --- 오른쪽 영역: 맵 그리드 ---
        EditorGUILayout.BeginVertical();
        GUILayout.Label("맵 배치 영역 (클릭: 배치, 우클릭: 삭제)", EditorStyles.boldLabel);

        if (gridBuilder != null)
        {
            DrawGridUI();
        }
        else
        {
            EditorGUILayout.HelpBox("Grid Builder 오브젝트를 먼저 할당해주세요.", MessageType.Warning);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridUI()
    {
        // 맵 영역 내부를 다시 가로로 정렬
        for (int y = gridBuilder.rows - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridBuilder.columns; x++)
            {
                int realIndex = x * gridBuilder.rows + y;
                bool isPlaced = gridData.ContainsKey(realIndex);
                string btnText = isPlaced ? gridData[realIndex].ToString() : "-";

                Color prevColor = GUI.backgroundColor;
                if (isPlaced) GUI.backgroundColor = Color.green;

                if (GUILayout.Button(btnText, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    if (Event.current.button == 0) gridData[realIndex] = selectedMonsterId;
                    else if (Event.current.button == 1) gridData.Remove(realIndex);
                }

                GUI.backgroundColor = prevColor;
            }
            GUILayout.EndHorizontal();
        }
    }

    private void SaveToScriptableObject()
    {
        if (gridBuilder == null) return;

        SO_WaveData newWave = ScriptableObject.CreateInstance<SO_WaveData>();
        newWave.waveIndex = this.waveIndex;
        newWave.spawns = new List<SpawnData>();

        foreach (var kvp in gridData)
        {
            newWave.spawns.Add(new SpawnData { spawnPointIndex = kvp.Key, monsterId = kvp.Value });
        }

        string folderPath = "Assets/Resources/WaveDatas";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string assetPath = $"{folderPath}/Wave_{waveIndex}.asset";
        AssetDatabase.CreateAsset(newWave, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"<color=cyan>[저장 완료]</color> {assetPath} 경로에 WaveData가 생성되었습니다!");
    }
}