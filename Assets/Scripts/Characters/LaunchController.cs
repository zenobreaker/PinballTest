using UnityEngine;

public class LaunchController : MonoBehaviour
{
    [Header("발사 설정")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float ballSpeed = 15f;

    [Header("조준선 설정")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int maxReflections = 3; 
    [SerializeField] private float maxStepDistance = 20f; 
    [SerializeField] private float minDragDistance = 0.5f; // 터치 민감도(데드존)

    [Header("조작 방식")]
    [SerializeField] private bool isSlingshotMode = true; // true: 당겨서 쏘기, false: 민 방향으로 쏘기

    private bool isAiming = false;
    private Vector2 aimDirection;
    private Vector2 touchStartWorldPos; // 터치를 시작한 월드 좌표

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // 1. 화면 터치 시작 (화면 어디든 상관없음)
        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
            // 터치 시작 지점의 월드 좌표 저장
            touchStartWorldPos = GetMouseWorldPosition();
        }

        // 2. 화면 드래그 중
        if (isAiming && Input.GetMouseButton(0))
        {
            Vector2 currentTouchWorldPos = GetMouseWorldPosition();
            
            // 터치 시작점과 현재 터치점의 차이(방향 벡터) 계산
            Vector2 dragVector = currentTouchWorldPos - touchStartWorldPos;

            // 터치를 살짝만 했을 때 조준선이 요동치는 것을 방지 (데드존)
            if (dragVector.magnitude > minDragDistance)
            {
                lineRenderer.enabled = true;

                // 조작 방식에 따라 방향 결정
                if (isSlingshotMode)
                {
                    // 앵그리버드처럼 뒤로 당기면 앞으로 날아가는 방식
                    aimDirection = -dragVector.normalized; 
                }
                else
                {
                    // 드래그한 방향 그대로 날아가는 방식
                    aimDirection = dragVector.normalized; 
                }

                // 캐릭터(launchPoint) 위치에서 계산된 방향으로 조준선 그리기
                DrawReflectionPattern(launchPoint.position, aimDirection);
            }
            else
            {
                // 드래그 거리가 짧으면 조준선을 숨김
                lineRenderer.enabled = false;
            }
        }

        // 3. 화면 터치 종료 (손가락을 뗐을 때 발사)
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            isAiming = false;
            lineRenderer.enabled = false;

            // 드래그를 충분히 했을 때만 발사되도록 처리
            if (aimDirection != Vector2.zero)
            {
                FireBall();
                aimDirection = Vector2.zero; // 발사 후 방향 초기화
            }
        }
    }

    // 마우스/터치 좌표를 2D 월드 좌표로 변환하는 헬퍼 함수
    private Vector2 GetMouseWorldPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(worldPos.x, worldPos.y);
    }

    private void DrawReflectionPattern(Vector2 startPos, Vector2 direction)
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPos);

        Vector2 currentStartPos = startPos;
        Vector2 currentDir = direction;

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentStartPos + currentDir * 0.01f, currentDir, maxStepDistance);

            if (hit.collider != null)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                currentDir = Vector2.Reflect(currentDir, hit.normal);
                currentStartPos = hit.point;

                if (hit.collider.CompareTag("Enemy")) break;
            }
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentStartPos + currentDir * maxStepDistance);
                break;
            }
        }
    }

    private void FireBall()
    {
        GameObject ball = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
        
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = aimDirection * ballSpeed;
        }
    }
}