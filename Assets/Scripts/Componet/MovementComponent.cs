using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementComponent : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 1.0f; // 실시간 이동 속도
    [SerializeField] private LayerMask obstacleLayer; // 다른 몬스터 레이어

    [Header("충돌 체크 설정")]
    [SerializeField] private float checkDistance = 0.05f; // 앞 유닛과 유지할 최소 간격 (바짝 붙게 하려면 작게 설정)

    private BoxCollider2D boxCollider;
    private CancellationTokenSource cts;
    private bool isStatusEffectRestricted = false;

    public float MoveSpeed { set { moveSpeed = value; } }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        MoveContinuousAsync(cts.Token).Forget();
    }

    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async UniTaskVoid MoveContinuousAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!isStatusEffectRestricted)
            {
                // 이번 프레임에 원래 이동해야 할 거리
                float desiredMoveDelta = moveSpeed * Time.deltaTime;

                // 실제 이동할 수 있는 허용 거리 계산
                float allowedMoveDelta = CalculateAllowedDistance(desiredMoveDelta);

                // 허용된 거리만큼만 아래로 이동
                if (allowedMoveDelta > 0)
                {
                    transform.Translate(Vector3.down * allowedMoveDelta);
                }
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private float CalculateAllowedDistance(float desiredDelta)
    {
        if (boxCollider == null) return desiredDelta;

        Vector2 origin = boxCollider.bounds.center;
        Vector2 size = boxCollider.bounds.size;

        // 원래 이동할 거리 + 여유 간격만큼 아래로 박스를 쏴서 확인합니다.
        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.down, desiredDelta + checkDistance, obstacleLayer);

        float minAllowedDistance = desiredDelta;

        foreach (var hit in hits)
        {
            // 자기 자신이 아닌 다른 방해물을 만났다면
            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
            {
                // 부딪히기 전까지 이동할 수 있는 빈 공간의 거리 (hit.distance)
                // 여기에 최소 유지 간격(checkDistance)을 빼서 바짝 붙도록 만듭니다.
                float allowed = hit.distance - checkDistance;
                allowed = Mathf.Max(0, allowed); // 뒤로 밀리지 않도록 최소값 0 보장

                // 가장 가까운 방해물을 기준으로 허용 거리를 깎습니다.
                if (allowed < minAllowedDistance)
                {
                    minAllowedDistance = allowed;
                }
            }
        }

        return minAllowedDistance;
    }

    public void SetMovementRestriction(bool isRestricted)
    {
        isStatusEffectRestricted = isRestricted;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        moveSpeed *= multiplier;
    }
}