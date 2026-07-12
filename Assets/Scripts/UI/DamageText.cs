using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class DamageText : MonoBehaviour
{

    TextMeshProUGUI text;
    private float lifeTime = 3.0f;
    private float currentTime;
    private Vector3 tdPos;
    [SerializeField] Color critColor;

    private const string BLEED_DMG_COLOR = "8A0A0A";
    private const string BURN_DMG_COLOR = "F65912";
    private const string POISON_DMG_COLOR = "588200";

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }



    private void LateUpdate()
    {
        tdPos += Vector3.up * Time.deltaTime;
        this.transform.position = Camera.main.WorldToScreenPoint(tdPos);

        currentTime -= Time.deltaTime;
        if (currentTime <= 0.0f)
        {
            currentTime = 0.0f;
            gameObject.SetActive(false);
        }
    }


    public void DrawDamage(Vector3 position, DamageEvent damageEvent)
    {
        DrawDamage(position, damageEvent.BaseDamage, damageEvent.isCrit);
    }

    public void DrawDamage(Vector3 position, float value, bool isCrit = false)
    {
        if (text == null) return;

        int finalValue = Mathf.RoundToInt(value);

        string colorTag = "FFFFFF";
        if (isCrit)
        {
            colorTag = ColorUtility.ToHtmlStringRGB(critColor);
            Debug.Log("is Critical");
        }

        text.text = $"<color=#{colorTag}>{finalValue}</color>";

        currentTime = lifeTime;

        tdPos = position;
        transform.position = Camera.main.WorldToScreenPoint(tdPos); ;

        gameObject.SetActive(true);

        transform.SetAsLastSibling();// 가장 앞에 그려지게 하기 위해 사용 
    }

    public void DrawDamage(Vector3 position, float value, DamageEvent evt)
    {
        if (text == null) return;

        int finalValue = Mathf.RoundToInt(value);
        bool isCrit = evt.isCrit;

        string colorTag = "FFFFFF";
        if (isCrit)
        {
            colorTag = ColorUtility.ToHtmlStringRGB(critColor);
        }


        if (evt.hitData.DamageType == DamageType.DOT_BLEED)
        {
            colorTag = BLEED_DMG_COLOR;
        }
        else if (evt.hitData.DamageType == DamageType.DOT_BURN)
        {
            colorTag = BURN_DMG_COLOR;
        }
        else if (evt.hitData.DamageType == DamageType.DOT_POISON)
        {
            colorTag = POISON_DMG_COLOR;
        }


        text.text = $"<color=#{colorTag}>{finalValue}</color>";

        currentTime = lifeTime;

        tdPos = position;
        transform.position = Camera.main.WorldToScreenPoint(tdPos);

        gameObject.SetActive(true);

        transform.SetAsLastSibling();
    }
}
