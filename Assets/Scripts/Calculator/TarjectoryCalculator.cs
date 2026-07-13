using System.Collections.Generic;
using UnityEngine;

public static class TrajectoryCalculator
{
    /// <summary>
    /// 레이캐스트를 쏴서 반사 궤적 좌표들을 계산해 반환합니다.
    /// 외부에서 객체 참조 없이 TrajectoryCalculator.CalculateTrajectory(...) 로 바로 호출합니다.
    /// </summary>
    public static List<Vector2> CalculateTrajectory(Vector2 startPos, Vector2 direction, int maxReflections, float maxStepDistance, LayerMask collisionLayer)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(startPos);

        Vector2 currentStartPos = startPos;
        Vector2 currentDir = direction;

        for (int i = 0; i < maxReflections; i++)
        {
            // 자기 자신과의 충돌을 피하기 위해 시작점에서 아주 살짝 띄워서 레이캐스트를 쏩니다.
            RaycastHit2D hit = Physics2D.Raycast(currentStartPos + currentDir * 0.01f, currentDir, maxStepDistance, collisionLayer);

            if (hit.collider != null)
            {
                points.Add(hit.point);

                // 반사각 계산
                currentDir = Vector2.Reflect(currentDir, hit.normal);
                currentStartPos = hit.point;
            }
            else
            {
                // 충돌체가 없다면 최대 거리만큼 선을 뻗고 종료
                points.Add(currentStartPos + currentDir * maxStepDistance);
                break;
            }
        }

        return points;
    }
}