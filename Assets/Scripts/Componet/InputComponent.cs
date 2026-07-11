using UnityEngine;
using UnityEngine.InputSystem;

public class InputComponent : MonoBehaviour
{
    [SerializeField] private TargetingComponent targetingComponent;

    private void Awake()
    {
        if (targetingComponent == null)
        {
            targetingComponent = GetComponent<TargetingComponent>();
        }
    }

    private void Update()
    {
        // 타게팅 컴포넌트가 없거나, 기기 입력이 없으면 리턴
        if (targetingComponent == null || Pointer.current == null) return;

        // 턴 개념(IsShooting 제한)을 삭제하여 공이 날아가는 도중에도 언제든 조준 가능
        if (Pointer.current.press.wasPressedThisFrame)
        {
            targetingComponent.StartAiming(GetPointerWorldPosition());
        }
        else if (Pointer.current.press.isPressed)
        {
            targetingComponent.UpdateAiming(GetPointerWorldPosition());
        }
        else if (Pointer.current.press.wasReleasedThisFrame)
        {
            targetingComponent.EndAiming();
        }
    }

    private Vector2 GetPointerWorldPosition()
    {
        Vector2 screenPos = Pointer.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return (Vector2)worldPos;
    }
}