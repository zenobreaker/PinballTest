using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionUI : MonoBehaviour
{
    [SerializeField] private List<CardBase> selectionButtons;
    [SerializeField] private CanvasGroup cg;

    public event Action OnClosePopup;
    private UniTaskCompletionSource completionSource;

    private void Awake()
    {
        if(cg != null && GameManager.Instance != null)
        {
            StageManager sm = GameManager.Instance.StageManager; 

            if(sm != null)
            {
                sm.SetSkillSelectionUI(this);
                gameObject.SetActive(false); 
            }
        }
    }

    // 반환 타입을 void에서 UniTask로 변경
    public async UniTask ShowAsync()
    {
        if (cg != null)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        // 1. SkillManager에서 무작위 3개 추출
        List<Skill> randomSkills = SkillManager.Instance.GetRandomAvailableSkills();

        if (randomSkills.Count == 0)
            return;

        gameObject.SetActive(true);

        // 2. 버튼(CardBase)에 스킬 정보
        for (int i = 0; i < randomSkills.Count; i++)
        {
            CardBase sb = selectionButtons[i];

            if (i < randomSkills.Count)
            {
                sb.Show(randomSkills[i]);
                sb.OnSelectSkill -= OnSelectSkill;
                sb.OnSelectSkill += OnSelectSkill;
            }
            else
                sb.gameObject.SetActive(false);
        }

        completionSource = new UniTaskCompletionSource();
        await completionSource.Task;
    }

    private void ClosePopup()
    {
        OnClosePopup?.Invoke();

        foreach (var card in selectionButtons)
            card.gameObject.SetActive(false);

        gameObject.SetActive(false);
        if (completionSource != null)
        {
            completionSource.TrySetResult();
            completionSource = null;
        }
    }

    public void OnSelectSkill(Skill selectedSkill)
    {
        SkillManager.Instance.SafeInvoke(v => v.AcquireSkill(selectedSkill));
        ClosePopup();
    }

}