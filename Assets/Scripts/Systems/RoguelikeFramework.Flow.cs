using UnityEngine;

public partial class RoguelikeFramework
{
    // 开发快捷：一键推进一小步，降低手动回归成本。
    private void DevAdvanceOneStep()
    {
        switch (state)
        {
            case RunState.Stage:
                StartPreparationForCurrentStage();
                battleLog = "[DEV] Stage -> Prepare";
                break;
            case RunState.Prepare:
                StartBattle();
                battleLog = "[DEV] Prepare -> Battle";
                break;
            case RunState.Battle:
                EndBattle(true);
                battleLog = "[DEV] Force Win -> Reward/Next";
                break;
            case RunState.Reward:
                if (currentRewardOffers.Count == 0) RollRewardOffers();
                PickReward(0);
                break;
            case RunState.Hex:
                if (currentHexOffers.Count == 0) RollHexOffers();
                PickHex(0);
                break;
            case RunState.GameOver:
                RestartRun();
                break;
        }
    }

    private void RestartRun()
    {
        stageIndex = 0;
        gold = 10;
        playerLevel = 1;
        exp = 0;
        playerLife = 30;
        winStreak = 0;
        loseStreak = 0;
        selectedHexes.Clear();
        currentRewardOffers.Clear();
        currentHexOffers.Clear();
        pendingHexAfterReward = false;
        benchUnits.Clear();
        deploySlots.Clear();
        benchUnits.Add(CreateUnit("soldier_sword", true));
        benchUnits.Add(CreateUnit("horse_raider", true));
        benchUnits.Add(CreateUnit("cannon_burst", true));
        state = RunState.Stage;
        battleLog = "新一轮开始";
        RedrawPrepareBoard();
    }

    // 最小自动回归：自动推进 3 关，校验核心状态机闭环不被改坏。
    private void DevRunRegression3Floors()
    {
        int startFloor = stageIndex;
        int target = Mathf.Min(stages.Count, startFloor + 3);
        int safety = 80;
        int steps = 0;
        bool blocked = false;

        while (stageIndex < target && state != RunState.GameOver && safety-- > 0)
        {
            steps++;
            switch (state)
            {
                case RunState.Stage:
                    StartPreparationForCurrentStage();
                    break;
                case RunState.Prepare:
                    if (deploySlots.Count == 0) AutoDeployFallback();
                    StartBattle();
                    // 回归模式下快速收敛，避免等待实时回合
                    if (state == RunState.Battle && battleStarted) EndBattle(true);
                    break;
                case RunState.Battle:
                    if (!battleStarted) { blocked = true; }
                    else EndBattle(true);
                    break;
                case RunState.Reward:
                    if (currentRewardOffers.Count == 0) RollRewardOffers();
                    PickReward(0);
                    break;
                case RunState.Hex:
                    if (currentHexOffers.Count == 0) RollHexOffers();
                    PickHex(0);
                    break;
                case RunState.GameOver:
                    blocked = true;
                    break;
            }

            if (blocked) break;
        }

        bool pass = stageIndex >= target;
        string result = pass
            ? $"[DEV] 3关回归通过 | {startFloor + 1}->{target} | steps:{steps} | life:{playerLife} gold:{gold}"
            : $"[DEV] 3关回归未通过 | state:{state} floor:{stageIndex + 1} target:{target} steps:{steps}";

        battleLog = result;
        Debug.Log(result);
    }


    #region Core Flow

    private void StartPreparationForCurrentStage()
    {
        if (stageIndex >= stages.Count)
        {
            state = RunState.GameOver;
            battleLog = "通关！你完成了线性章节。";
            return;
        }

        state = RunState.Prepare;
        battleStarted = false;
        inspectedUnit = null;
        showTooltip = false;

        int roundBaseGold = 5;
        int streakGold = winStreak >= 2 ? Mathf.Min(3, winStreak / 2) : (loseStreak >= 2 ? Mathf.Min(2, loseStreak / 2) : 0);
        int interest = Mathf.Min(GetInterestCap(), gold / 10);
        int hexBonus = HasHex("rich") ? 4 : 0;

        gold += roundBaseGold + streakGold + interest + hexBonus;

        int expGain = 2 + (HasHex("fast_train") ? 2 : 0);
        GainExp(expGain);

        if (HasHex("healing"))
        {
            foreach (var u in deploySlots)
            {
                u.hp = Mathf.Min(u.maxHp, u.hp + Mathf.RoundToInt(u.maxHp * 0.2f));
            }
        }

        RefreshShop(true);
        AutoMergeAll();
        RedrawPrepareBoard();

        var st = stages[stageIndex];
        battleLog = $"准备阶段：第{st.floor}关({st.type}) | +{roundBaseGold}+利息{interest}+连胜/败{streakGold}";
    }

    private void StartBattle()
    {
        if (stageIndex >= stages.Count) return;

        var st = stages[stageIndex];
        state = RunState.Battle;
        battleStarted = true;
        battleStartedTurn = 0;
        turnIndex = 0;
        inspectedUnit = null;
        showTooltip = false;
        battleLog = $"战斗开始：第{st.floor}关 {st.type}";

        playerUnits.Clear();
        enemyUnits.Clear();
        ClearViews();
        DrawBoard();

        // 玩家单位
        if (deploySlots.Count == 0)
        {
            AutoDeployFallback();
        }

        int[,] fallbackPos = { { 0, 1 }, { 0, 2 }, { 0, 3 }, { 1, 1 }, { 1, 2 }, { 1, 3 }, { 2, 2 }, { 2, 3 } };
        int maxDeploy = GetBoardCap();
        for (int i = 0; i < deploySlots.Count && i < maxDeploy; i++)
        {
            var u = CloneUnit(deploySlots[i]);
            u.player = true;
            u.usedCharge = false;
            if (u.x < 0 || u.x >= W || u.y < 0 || u.y >= H)
            {
                u.x = fallbackPos[i, 0];
                u.y = fallbackPos[i, 1];
            }
            else
            {
                // 准备阶段直接在战场坐标布阵
                u.y = Mathf.Clamp(u.y, 0, H - 1);
            }
            playerUnits.Add(u);
        }

        SpawnEnemiesForStage(st);

        CreateViews(playerUnits, new Color(0.2f, 0.7f, 1f));
        CreateViews(enemyUnits, new Color(0.95f, 0.35f, 0.4f));
        RefreshViews();
    }

    private void SpawnEnemiesForStage(StageNode st)
    {
        int enemyCount = Mathf.Clamp(2 + st.power, 3, 7);
        int[,] pos = { { 7, 2 }, { 8, 1 }, { 8, 2 }, { 8, 3 }, { 9, 1 }, { 9, 2 }, { 9, 4 }, { 7, 4 } };

        for (int i = 0; i < enemyCount; i++)
        {
            string key = PickEnemyUnitKey(st);
            var u = CreateUnit(key, false);

            float hpScale = 1f + (st.power - 1) * 0.15f;
            float atkScale = 1f + (st.power - 1) * 0.11f;
            u.hp = Mathf.RoundToInt(u.hp * hpScale);
            u.maxHp = u.hp;
            u.atk = Mathf.RoundToInt(u.atk * atkScale);
            u.spd += Mathf.FloorToInt((st.power - 1) * 0.5f);

            if (st.type == StageType.Elite || st.type == StageType.Boss || UnityEngine.Random.value < Mathf.Clamp01((st.floor - 2) * 0.12f))
            {
                UpgradeUnit(u);
                if (st.type == StageType.Boss && UnityEngine.Random.value < 0.45f) UpgradeUnit(u); // 有机会3星
            }

            u.x = pos[i, 0];
            u.y = pos[i, 1];
            enemyUnits.Add(u);
        }
    }

    private string PickEnemyUnitKey(StageNode st)
    {
        if (st.type == StageType.Boss)
        {
            string[] bossLike = { "chariot_tank", "cannon_missile", "horse_nightmare", "general_fire" };
            return bossLike[UnityEngine.Random.Range(0, bossLike.Length)];
        }
        return basePool[UnityEngine.Random.Range(0, basePool.Count)];
    }

    private string BuildBattleOutcomeDetail(bool win)
    {
        int allyAlive = playerUnits.FindAll(u => u.Alive).Count;
        int enemyAlive = enemyUnits.FindAll(u => u.Alive).Count;

        if (win)
        {
            return $"我方存活:{allyAlive} 敌方存活:{enemyAlive}";
        }

        int enemyHpLeft = 0;
        Unit topThreat = null;
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            var e = enemyUnits[i];
            if (!e.Alive) continue;
            enemyHpLeft += Mathf.Max(0, e.hp);
            if (topThreat == null || e.damageDealt > topThreat.damageDealt) topThreat = e;
        }

        string threatTxt = topThreat == null
            ? "关键威胁:无"
            : $"关键威胁:{topThreat.Name} 造成{topThreat.damageDealt}伤害";

        return $"我方存活:{allyAlive} 敌方存活:{enemyAlive} 敌方剩余生命:{enemyHpLeft} | {threatTxt}";
    }

    private void EndBattle(bool win)
    {
        battleStarted = false;

        if (win)
        {
            winStreak++;
            loseStreak = 0;
            int reward = 8 + Mathf.Min(5, stages[stageIndex].power);
            gold += reward;
            battleLog = $"胜利！+{reward}金币 | {BuildBattleOutcomeDetail(true)}";
        }
        else
        {
            loseStreak++;
            winStreak = 0;
            int reward = 4;
            gold += reward;

            int lifeLoss = Mathf.Clamp(2 + stages[stageIndex].power, 2, 12);
            playerLife -= lifeLoss;
            battleLog = $"失败，保底 +{reward}金币 | 生命 -{lifeLoss} | {BuildBattleOutcomeDetail(false)}";

            if (playerLife <= 0)
            {
                playerLife = 0;
                state = RunState.GameOver;
                battleLog += " | 生命耗尽，挑战结束";
                return;
            }
        }

        pendingHexAfterReward = stages[stageIndex].giveHex;
        stageIndex++;

        if (stageIndex >= stages.Count)
        {
            state = RunState.GameOver;
            battleLog += " | 章节结束";
            return;
        }

        RollRewardOffers();
        state = RunState.Reward;
        battleLog += " | 战后奖励三选一";
    }

    #endregion

}
