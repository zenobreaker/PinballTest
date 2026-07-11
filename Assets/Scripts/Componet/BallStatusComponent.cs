using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BallType
{
    Normal, 
    Fire, 
    Ice,
    Laser,
    Ghost,
    Cluster,
}

public class BallModifier
{
    float AditionalDamage = 0.0f; // 추가 피해량 
    float BonusHitDamage = 0.0f; // 타격 피해량  
    float BonusCritChance = 0.0f;
}

public class BallStatusComponent : MonoBehaviour
{
    Dictionary<BallType, BallModifier> ballModifiers  = new();

    private void Awake()
    {
        ballModifiers.Add(BallType.Normal, new BallModifier());
    }
}
