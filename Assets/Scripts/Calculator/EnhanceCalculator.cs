using System.Collections.Generic;
using UnityEngine;

public static class EnhanceCalculator 
{
    private static readonly Dictionary<ItemRank, float> getMulitplier = new()
    {
        {ItemRank.COMMON, 1.0f},
        {ItemRank.MAGIC, 1.2f},
        {ItemRank.RARE, 1.5f },
        {ItemRank.UNIQUE, 2.0f},
        {ItemRank.LEGENDARY, 3.0f},
    };

    private const int BASE_COST = 100;
    private const float GROWTH_RATE = 0.25f;

    private static readonly Dictionary<int, float> extraMultipliers=new()
    {
        {10, 1.5f},
        {13, 2.0f},
        {15, 3.0f},
    };

}
