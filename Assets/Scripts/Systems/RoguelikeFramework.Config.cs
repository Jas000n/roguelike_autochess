using System.Collections.Generic;
using UnityEngine;

public partial class RoguelikeFramework
{
    // Stage A2: 轻量配置数据层（先以内置配置集中管理，后续可平滑迁移到 ScriptableObject/JSON）
    private static readonly Dictionary<int, float[]> ShopCostOddsByLevelConfig = new()
    {
        { 1, new[] { 0f, 0.70f, 0.30f, 0f, 0f, 0f } },
        { 2, new[] { 0f, 0.55f, 0.35f, 0.10f, 0f, 0f } },
        { 3, new[] { 0f, 0.40f, 0.40f, 0.18f, 0.02f, 0f } },
        { 4, new[] { 0f, 0.25f, 0.40f, 0.28f, 0.07f, 0f } },
        { 5, new[] { 0f, 0.18f, 0.30f, 0.35f, 0.15f, 0.02f } },
        { 6, new[] { 0f, 0.12f, 0.22f, 0.38f, 0.23f, 0.05f } },
        { 7, new[] { 0f, 0.08f, 0.16f, 0.34f, 0.30f, 0.12f } },
        { 8, new[] { 0f, 0.05f, 0.10f, 0.25f, 0.40f, 0.20f } }
    };

    private float GetOpeningUnitCostWeight(int cost)
    {
        return cost switch
        {
            1 => 1.15f,
            2 => 1.0f,
            3 => 0.7f,
            _ => 0.2f
        };
    }

    private float[] GetShopCostOddsConfig(int level)
    {
        int lv = Mathf.Clamp(level, 1, 8);
        return ShopCostOddsByLevelConfig.TryGetValue(lv, out var odds)
            ? odds
            : ShopCostOddsByLevelConfig[8];
    }

    private float GetLockedCompClassBiasByLevel(int level)
    {
        return level <= 3 ? 2.2f : level <= 5 ? 1.8f : 1.4f;
    }

    private const float LockedCompOriginBias = 0.9f;
    private const float Double4LowCostBonus = 0.5f;
    private const float LateGameHighCostBonus = 0.7f;
    private const float EarlyGameHighCostPenalty = 0.65f;
}
