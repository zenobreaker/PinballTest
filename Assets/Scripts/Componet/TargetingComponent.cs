using System.Collections.Generic;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    [Header("연결된 컴포넌트")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform launchPoint;

    [Header("조작 설정")]
    [SerializeField] private float minDragDistance = 0.5f;
    [SerializeField] private bool isSlingshotMode = true;

    [Header("궤적 연산 설정")]
    [SerializeField] private int maxReflections = 3;
    [SerializeField] private float maxStepDistance = 20f;
    [SerializeField] private LayerMask collisionLayer;

    private bool isAiming = false;
    private Vector2 aimDirection;
    private Vector2 touchStartPos;

    // InputComponent에서 호출됨
    public void StartAiming(Vector2 startWorldPos)
    {
        isAiming = true;
        touchStartPos = startWorldPos;
    }

    // InputComponent에서 호출됨
    public void UpdateAiming(Vector2 currentWorldPos)
    {
        if (!isAiming) return;

        Vector2 dragVector = currentWorldPos - touchStartPos;

        if (dragVector.magnitude > minDragDistance)
        {
            if (lineRenderer != null)
                lineRenderer.enabled = true;

            // 드래그 방식에 따른 조준 방향 설정
            aimDirection = isSlingshotMode ? -dragVector.normalized : dragVector.normalized;


            // 핵심: 조준하는 즉시 BallManager의 다음 발사 타겟팅 방향을 갱신합니다.
            if (BallManager.Instance != null)
            {
                BallManager.Instance.SetAimDirection(aimDirection);
            }

            // 1. 계산기에게 궤적 연산 요청
            // 스태틱 클래스 직접 호출 (객체 참조 불필요)
            List<Vector2> trajectoryPoints = TrajectoryCalculator.CalculateTrajectory(
                launchPoint.position,
                aimDirection,
                maxReflections,
                maxStepDistance,
                collisionLayer
            );

            // 2. 받아온 좌표들로 조준선 그리기
            DrawTrajectory(trajectoryPoints);
        }
        else
        {
            // 데드존 이내일 경우 조준선 숨김 및 방향 초기화
            if (lineRenderer != null)
                lineRenderer.enabled = false;
            aimDirection = Vector2.zero;
        }
    }

    // InputComponent에서 호출됨
    public void EndAiming()
    {
        if (!isAiming) return;

        isAiming = false;

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    // LineRenderer 업데이트 전담
    private void DrawTrajectory(List<Vector2> points)
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }
    }
}