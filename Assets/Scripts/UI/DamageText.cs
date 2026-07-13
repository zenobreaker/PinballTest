using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float lifeTime = 1.0f; // 텍스트 유지 시간 (너무 길면 화면에 남으니 1초 정도로 줄이는 걸 추천!)
    private float currentTime;
    private Vector3 tdPos;

    [SerializeField] Color critColor = Color.yellow;
    [SerializeField] private float floatSpeed = 0.1f; // 💡 위로 올라가는 속도 (에디터에서 조절하세요!)

    private const string BLEED_DMG_COLOR = "8A0A0A";
    private const string BURN_DMG_COLOR = "F65912";
    private const string POISON_DMG_COLOR = "588200";

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        // 1. 월드 좌표에서 위로 조금씩 이동
        tdPos += (Vector3.up ) *floatSpeed * Time.deltaTime;

        // 2. 스크린 좌표로 변환 후 Z축을 0으로 강제 고정!
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tdPos);
        screenPos.z = 0f;

        transform.position = screenPos;

        // 3. 시간 체크
        currentTime -= Time.deltaTime;
        if (currentTime <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void DrawDamage(Vector3 position, DamageEvent damageEvent)
    {
        DrawDamage(position, damageEvent.BaseDamage, damageEvent);
    }

    public void DrawDamage(Vector3 position, float value, bool isCrit = false)
    {
        if (text == null) return;

        int finalValue = Mathf.RoundToInt(value);
        string colorTag = "FFFFFF";

        if (isCrit)
        {
            colorTag = ColorUtility.ToHtmlStringRGB(critColor);
        }

        text.text = $"<color=#{colorTag}>{finalValue}</color>";

        InitTransform(position);
    }

    public void DrawDamage(Vector3 position, float value, DamageEvent evt)
    {
        if (text == null) return;

        int finalValue = Mathf.RoundToInt(value);
        string colorTag = "FFFFFF";

        if (evt.isCrit)
            colorTag = ColorUtility.ToHtmlStringRGB(critColor);
        else if (evt.hitData.DamageType == DamageType.DOT_BLEED)
            colorTag = BLEED_DMG_COLOR;
        else if (evt.hitData.DamageType == DamageType.DOT_BURN)
            colorTag = BURN_DMG_COLOR;
        else if (evt.hitData.DamageType == DamageType.DOT_POISON)
            colorTag = POISON_DMG_COLOR;

        text.text = $"<color=#{colorTag}>{finalValue}</color>";

        InitTransform(position);
    }

    private void InitTransform(Vector3 position)
    {
        // 타격 지점 설정 
        float randomOffsetX = Random.Range(-0.3f, 0.3f);
        tdPos = position + new Vector3(randomOffsetX, -0.5f, 0f); // 머리 위쪽에서 뜨도록 약간 위(0.5f)로 보정

        // 스크린 좌표 변환 및 Z축 0 고정
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tdPos);
        screenPos.z = 0f;
        transform.position = screenPos;

        currentTime = lifeTime;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
}