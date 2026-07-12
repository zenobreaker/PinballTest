using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionUI : MonoBehaviour
{
    [SerializeField] private List<CardBase> selectionButtons; 

    public event Action OnClosePopup;
    private UniTaskCompletionSource completionSource;

    // 반환 타입을 void에서 UniTask로 변경
    public async UniTask ShowAsync()
    {
        gameObject.SetActive(true);
        

        // 1. SkillManager에서 무작위 3개 추출
         List<Skill> randomSkills = SkillManager.Instance.GetRandomAvailableSkills();
        
        
        // 2. 버튼(CardBase)에 스킬 정보
        for (int i = 0; i < randomSkills.Count; i++) 
        {
            CardBase sb = selectionButtons[i];
            sb.Show(randomSkills[i]);
            sb.OnSelectSkill += OnSelectSkill;
        }

        // 새로운 CompletionSource를 생성합니다.
        completionSource = new UniTaskCompletionSource();

        // 유저가 스킬을 선택해서 completionSource.TrySetResult()가 호출될 때까지 여기서 대기합니다.
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