using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpGroup : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;
    [SerializeField] private TMP_Text levelText;

    private void OnEnable()
    {
        if (ExperienceManager.Instance == null) return;

        ExperienceManager.Instance.OnLevelUp_One += SetLevel;
        ExperienceManager.Instance.OnChangeExp += SetValue;
    }
    private void OnDisable()
    {
        if (ExperienceManager.Instance == null) return; 

        ExperienceManager.Instance.OnLevelUp_One -= SetLevel;
        ExperienceManager.Instance.OnChangeExp -= SetValue;
    }

    public void SetValue(float value, float maxValue)
    {
        gaugeImage.fillAmount = value / maxValue;
    }

    public void SetLevel(int level)
    {
        if(levelText != null) 
            levelText.text = level.ToString();
    }
}
