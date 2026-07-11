using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridBuilder))]
public class GridBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // 기존 인스펙터 UI(변수들)를 그대로 그립니다.

        GridBuilder gridBuilder = (GridBuilder)target;

        EditorGUILayout.Space(10); // 여백 추가

        // 생성 버튼
        if (GUILayout.Button("전체 영역에 오브젝트 생성", GUILayout.Height(40)))
        {
            GenerateAllObjects(gridBuilder);
        }

        EditorGUILayout.Space(5);

        // 삭제 버튼 (편의 기능)
        if (GUILayout.Button("생성된 오브젝트 전체 삭제", GUILayout.Height(30)))
        {
            ClearAllObjects(gridBuilder);
        }
    }

    private void GenerateAllObjects(GridBuilder gridBuilder)
    {
        if (gridBuilder.prefabToPlace == null)
        {
            Debug.LogWarning("배치할 프리팹(Prefab To Place)을 먼저 할당해주세요.");
            return;
        }

        float gridWidth = (gridBuilder.columns - 1) * gridBuilder.cellSize.x;
        float gridHeight = (gridBuilder.rows - 1) * gridBuilder.cellSize.y;
        Vector3 startPos = gridBuilder.transform.position - new Vector3(gridWidth / 2f, gridHeight / 2f, 0) + (Vector3)gridBuilder.offset;

        int createCount = 0;

        for (int x = 0; x < gridBuilder.columns; x++)
        {
            for (int y = 0; y < gridBuilder.rows; y++)
            {
                Vector3 nodePos = startPos + new Vector3(x * gridBuilder.cellSize.x, y * gridBuilder.cellSize.y, 0);

                // 중복 생성 방지: 해당 위치에 이미 오브젝트가 있는지 검사
                Collider2D hit = Physics2D.OverlapCircle(nodePos, 0.1f);
                if (hit != null && hit.transform.parent == gridBuilder.transform)
                {
                    continue; // 이미 있다면 건너뜀
                }

                // 프리팹 생성
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(gridBuilder.prefabToPlace);
                newObj.transform.position = nodePos;
                newObj.transform.SetParent(gridBuilder.transform);

                // Ctrl+Z (실행 취소) 기록
                Undo.RegisterCreatedObjectUndo(newObj, "Generate All Objects");
                createCount++;
            }
        }

        Debug.Log($"총 {createCount}개의 오브젝트가 생성되었습니다.");
    }

    private void ClearAllObjects(GridBuilder gridBuilder)
    {
        // Ctrl+Z를 지원하기 위해 안전하게 자식 오브젝트 삭제
        int childCount = gridBuilder.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(gridBuilder.transform.GetChild(i).gameObject);
        }
        Debug.Log("생성된 모든 자식 오브젝트를 삭제했습니다.");
    }
}