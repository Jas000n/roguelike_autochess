using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopOddsConfig", menuName = "DragonChessLegends/Config/Shop Odds")]
public class ShopOddsConfigAsset : ScriptableObject
{
    [Serializable]
    public class LevelOddsEntry
    {
        [Range(1, 8)] public int level = 1;
        [Tooltip("索引0保留不用，1~5分别是1费到5费概率")]
        public float[] odds = new float[6] { 0f, 0.70f, 0.30f, 0f, 0f, 0f };
    }

    public List<LevelOddsEntry> levels = new();

    [SerializeField, TextArea(2, 5)]
    private string inspectorValidationHint = "";

    private void OnValidate()
    {
        levels ??= new List<LevelOddsEntry>();
        levels.Sort((a, b) =>
        {
            if (ReferenceEquals(a, b)) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.level.CompareTo(b.level);
        });

        var seen = new HashSet<int>();
        var dup = new List<int>();
        foreach (var e in levels)
        {
            if (e == null) continue;
            if (!seen.Add(e.level) && !dup.Contains(e.level)) dup.Add(e.level);
        }

        var missing = new List<int>();
        for (int lv = 1; lv <= 8; lv++) if (!seen.Contains(lv)) missing.Add(lv);

        if (dup.Count == 0 && missing.Count == 0) inspectorValidationHint = "OK: 覆盖 level 1~8，且无重复。";
        else
        {
            string dupText = dup.Count == 0 ? "无" : string.Join(",", dup);
            string missText = missing.Count == 0 ? "无" : string.Join(",", missing);
            inspectorValidationHint = $"重复 level: {dupText} | 缺失 level: {missText}";
        }
    }

    public bool TryBuildRuntimeMap(out Dictionary<int, float[]> map, out string error)
    {
        map = new Dictionary<int, float[]>();
        error = "";

        if (levels == null || levels.Count == 0)
        {
            error = "ShopOddsConfigAsset.levels 为空";
            return false;
        }

        foreach (var entry in levels)
        {
            if (entry == null)
            {
                error = "ShopOddsConfigAsset.levels 存在空 entry";
                return false;
            }
            if (entry.level < 1 || entry.level > 8)
            {
                error = $"ShopOddsConfigAsset level 非法: {entry.level}";
                return false;
            }
            if (entry.odds == null || entry.odds.Length != 6)
            {
                error = $"ShopOddsConfigAsset[{entry.level}] odds 长度非法（期望6）";
                return false;
            }

            float sum = 0f;
            for (int i = 0; i < entry.odds.Length; i++)
            {
                if (entry.odds[i] < 0f)
                {
                    error = $"ShopOddsConfigAsset[{entry.level}] odds[{i}] 为负数";
                    return false;
                }
                sum += entry.odds[i];
            }

            if (Mathf.Abs(sum - 1f) > 0.01f)
            {
                error = $"ShopOddsConfigAsset[{entry.level}] 概率和异常: {sum:F3}";
                return false;
            }

            if (map.ContainsKey(entry.level))
            {
                error = $"ShopOddsConfigAsset 存在重复 level: {entry.level}";
                return false;
            }

            var copy = new float[6];
            Array.Copy(entry.odds, copy, 6);
            map[entry.level] = copy;
        }

        return true;
    }
}
