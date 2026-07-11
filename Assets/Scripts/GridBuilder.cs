using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [Header("격자 설정")]
    public int columns = 7; // 가로 칸 수
    public int rows = 9;    // 세로 칸 수
    public Vector2 cellSize = new Vector2(1f, 1f); // 타일 한 칸의 크기
    public Vector2 offset = Vector2.zero; // 전체 격자의 미세 조정 오프셋

    [Header("배치할 오브젝트")]
    public GameObject prefabToPlace; // 버튼 클릭 시 생성할 프리팹

    // 씬 뷰에서 격자 위치를 시각적으로 보여줍니다.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        float gridWidth = (columns - 1) * cellSize.x;
        float gridHeight = (rows - 1) * cellSize.y;
        Vector3 startPos = transform.position - new Vector3(gridWidth / 2f, gridHeight / 2f, 0) + (Vector3)offset;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 nodePos = startPos + new Vector3(x * cellSize.x, y * cellSize.y, 0);

                // 마름모 모양 대신 가벼운 구형으로 시각적 가이드만 표시
                Gizmos.DrawSphere(nodePos, 0.1f);
            }
        }
    }
}