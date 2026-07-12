using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private TMP_Text skillNameText;
    [SerializeField] private TMP_Text skillDescText;
    [SerializeField] private TMP_Text skillLevelText;
    [SerializeField] private Button selectButton; 

    private Skill currentSkill;

    public event Action OnSelectSkill;

    private void Awake()
    {
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectSkill);
    }

    public void Show(Skill skill)
    {
        if (skill == null) return;
        
        currentSkill = skill; 

        if (skillIcon != null)
            skillIcon.sprite = skill.Icon;

        if (skillNameText != null)
            skillNameText.text = skill.Name;

        if (skillDescText != null)
            skillDescText.text = skill.Description;

        if (skillLevelText != null)
            skillLevelText.text = "Lv. " + skill.SkillLevel.ToString();
    }


    private void SelectSkill()
    {
        if(currentSkill is ActiveSkill ball)
        {
            SkillManager.Instance.SafeInvoke(v => v.AcquireSkill(currentSkill));
            
            OnSelectSkill?.Invoke(); 
        }
    }
}
