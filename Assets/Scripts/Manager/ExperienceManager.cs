using System;
using UnityEngine;

public class ExperienceManager : Singleton<ExperienceManager>
{

    private int level = 1;
    private int currentExp;
    private int needExp = 2;

    public event Action OnLevelUp;
    public event Action<int> OnLevelUp_One;
    public event Action<float, float> OnChangeExp;

    protected override void Awake()
    {
        base.Awake();
        if(GameManager.Instance != null)
            GameManager.Instance.OnBeginStage += Init;
    }

    public void Init()
    {
        level = 1;
        currentExp = 0;
        needExp = GetNeedExp(level);

        OnChangeExp?.Invoke(currentExp, needExp);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= needExp)
        {
            currentExp = 0;
            level++;

            needExp = GetNeedExp(level);

            OnLevelUp?.Invoke();
            OnLevelUp_One?.Invoke(level);
        }

        OnChangeExp?.Invoke(currentExp, needExp);
    }

    private int GetNeedExp(int level)
    {
        return (int)Mathf.Round(level * 1.5f);
    }
}
