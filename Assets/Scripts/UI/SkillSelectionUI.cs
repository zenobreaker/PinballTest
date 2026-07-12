using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionUI : MonoBehaviour
{
    [SerializeField] private List<CardBase> selectionButtons; // 3개 버튼

    public event Action OnClosePopup;

    public void ShowSelectionPopup()
    {
        // 1. SkillDatabase에서 무작위 3개 추출 (보유 여부 및 최대 레벨 체크)
        // 2. 버튼에 스킬 정보(이름, 효과, 레벨) 표시
    }

    private void ClosePopup()
    {
        OnClosePopup?.Invoke();

        gameObject.SetActive(false); 
    }

    public void OnSelectSkill(Skill selectedSkill)
    {
        SkillManager.Instance.SafeInvoke(v => v.AcquireSkill(selectedSkill)); 
        ClosePopup();
    }
}