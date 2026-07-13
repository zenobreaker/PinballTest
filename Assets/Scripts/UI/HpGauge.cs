using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpGauge : MonoBehaviour
{
    [SerializeField] private Image gauge;
    [SerializeField] private TMP_Text hpText;


    public void DrawHP(float hp, float maxHP)
    {
        if (gauge != null)
        {
            gauge.fillAmount = hp / maxHP;
        }
        if (hpText != null)
            hpText.text = hp.ToString();
    }
}
