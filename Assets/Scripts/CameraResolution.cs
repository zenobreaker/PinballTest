using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [Header("기준 해상도 (기획 기준)")]
    [SerializeField] private float targetWidth = 1080f;
    [SerializeField] private float targetHeight = 1920f;

    private void Awake()
    {
        AdjustCameraSize();
    }

    private void AdjustCameraSize()
    {
        if (!TryGetComponent<Camera>(out var cam)) return;

        // 기기의 현재 화면 비율
        float screenRatio = (float)Screen.width / Screen.height;
        // 우리가 맞추고자 하는 타겟 화면 비율 (예: 16:9)
        float targetRatio = targetWidth / targetHeight;

        // 현재 화면이 타겟 비율보다 더 길쭉하다면 (요즘 스마트폰 20:9 등)
        if (screenRatio < targetRatio)
        {
            // 가로(Width)가 잘리지 않도록 카메라의 Orthographic Size를 키워줍니다.
            cam.orthographicSize = cam.orthographicSize * (targetRatio / screenRatio);
        }
        // 화면이 더 넓적하다면 (태블릿 등) 기본 세로 고정 상태를 유지합니다.
    }
}