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

    public event Action<Skill> OnSelectSkill;

    private void Awake()
    {
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectSkill);
    }

    public void Show(Skill skill)
    {
        if (skill == null) return;

        gameObject.SetActive(true);

        currentSkill = skill;

        bool isOwned = SkillManager.Instance.HasSkill(skill.SkillID);
        int targetLevel = isOwned ? (skill.SkillLevel + 1) : 1;

        if (skillIcon != null)
            skillIcon.sprite = skill.Icon;

        if (skillNameText != null)
            skillNameText.text = skill.Name;

        if (skillDescText != null)
            skillDescText.text = skill.GetDesc(targetLevel);

        if (skillLevelText != null)
            skillLevelText.text = "Lv. " + targetLevel.ToString();
    }


    private void SelectSkill()
    {
        OnSelectSkill?.Invoke(currentSkill);
    }
}
