using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public partial class RoguelikeFramework
{
    private void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"+$ {amount}");
        UpdateUI();
    }

    private int CalculateInterest()
    {
        return Mathf.Min(5, gold / 10);
    }

    private void RefreshShop(bool manualRefresh = true)
    {
        if (manualRefresh && gold < refreshCost)
        {
            Debug.Log("Not enough gold.");
            return;
        }

        if (manualRefresh) gold -= refreshCost;

        shopOffers.Clear();
        int tierRates = playerLevel; // 简易按等级提升星级概率
        for (int i = 0; i < 5; i++)
        {
            var allowedPool = unitRegistry.Values.Where(u => u.cost <= tierRates).ToList();
            if (allowedPool.Count > 0)
            {
                shopOffers.Add(allowedPool[Random.Range(0, allowedPool.Count)]);
            }
        }
        UpdateUI();
    }

    private void BuyUnit(int index)
    {
        if (index < 0 || index >= shopOffers.Count) return;
        var unitDef = shopOffers[index];
        if (gold < unitDef.cost)
        {
            Debug.Log("Not enough gold.");
            return;
        }
        if (bench.Count >= benchSize)
        {
            Debug.Log("Bench full.");
            return;
        }

        gold -= unitDef.cost;
        Unit newUnit = new Unit
        {
            id = System.Guid.NewGuid().ToString(),
            def = unitDef,
            hp = unitDef.hp,
            maxHp = unitDef.hp,
            atk = unitDef.atk,
            spd = unitDef.spd,
            range = unitDef.range,
            star = 1,
            player = true
        };

        bench.Add(newUnit);
        shopOffers.RemoveAt(index);
        CheckAutoMerge(unitDef.key);

        UpdateUI();
    }

    private void SellUnit(Unit u)
    {
        if (bench.Contains(u))
        {
            bench.Remove(u);
            gold += u.def.cost * u.star;
        }
        else if (boardPlayer.Contains(u))
        {
            boardPlayer.Remove(u);
            gold += u.def.cost * u.star;
        }
        UpdateUI();
        CalculateTraits();
    }

    private void BuyExp()
    {
        int cost = 4;
        if (gold >= cost && playerLevel < maxLevel)
        {
            gold -= cost;
            playerExp += cost;
            int req = LevelReq(playerLevel);
            if (playerExp >= req)
            {
                playerExp -= req;
                playerLevel++;
                Debug.Log($"Level UP! -> {playerLevel}");
            }
            UpdateUI();
        }
    }

    private void CheckAutoMerge(string key)
    {
        var allMyUnits = new List<Unit>();
        allMyUnits.AddRange(bench);
        allMyUnits.AddRange(boardPlayer);

        // check merge for star 2
        var tier2List = allMyUnits.Where(u => u.def.key == key && u.star == 2).ToList();
        if (tier2List.Count >= 3)
        {
            MergeUnits(tier2List, 3);
            return;
        }

        // check merge for star 1
        var tier1List = allMyUnits.Where(u => u.def.key == key && u.star == 1).ToList();
        if (tier1List.Count >= 3)
        {
            MergeUnits(tier1List, 2);
        }
    }

    private void MergeUnits(List<Unit> targetUnits, int targetStar)
    {
        Unit mainUnit = targetUnits[0];
        mainUnit.star = targetStar;
        mainUnit.hp = (int)(mainUnit.def.hp * Mathf.Pow(1.8f, targetStar - 1));
        mainUnit.maxHp = mainUnit.hp;
        mainUnit.atk = (int)(mainUnit.def.atk * Mathf.Pow(1.5f, targetStar - 1));

        for (int i = 1; i < 3; i++)
        {
            var u = targetUnits[i];
            if (bench.Contains(u)) bench.Remove(u);
            if (boardPlayer.Contains(u)) boardPlayer.Remove(u);
        }

        Debug.Log($"Merged into Star {targetStar} {mainUnit.def.name}!");
        UpdateUI();
    }
}
