using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;



public class AIDirectorSystem : AbstractSystem
{
    Storage storage;
    GameCoreModel gameCoreModel;
    GameEntityModel gameEntityModel;

    /// <summary>
    /// 游戏默认的基础战斗难度配置
    /// </summary>
    BattleDifficultyConfig battleDifficultyConfig;

    // 战斗流程状态管理，但是目前设计为同时只能在一个HexCell进行战斗
    private Dictionary<HexCell, BattleFlowState> activeBattles = new();

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();
        gameCoreModel = this.GetModel<GameCoreModel>();
        gameEntityModel = this.GetModel<GameEntityModel>();
        battleDifficultyConfig = storage.GetBattleDifficultyConfig();

        // Region战斗开始
        this.RegisterEvent<OnRegionEnterTriggerEvent>(OnBattleRegionTrigger);
        // 敌人死亡事件监听
        this.RegisterEvent<EnemyDefeatedEvent>(OnEnemyDefeated);
    }

    #region 监听事件

    private void OnBattleRegionTrigger(OnRegionEnterTriggerEvent evt)
    {
        HexCell hexCell = evt.hexCell;
        
        // 已经发现了，不在触发
        if (gameEntityModel.GetHexCellBattleState(hexCell) != HexCellState.NoDiscover)
        {
            Debug.Log($"再次进入已发现区域（{hexCell.coord.x}, {hexCell.coord.y}），不触发战斗");
            return;
        }

        // 是初始中心，拿到元素即可。没有战斗
        if (hexCell.IsRealmInitCenter)
        {
            Debug.Log($"玩家进入领域初始区域（{hexCell.coord.x}, {hexCell.coord.y}）");
            gameEntityModel.SetHexCellBattleState(hexCell, HexCellState.AllCompleted);
            return;
        }
        else // 是战斗区域，从Model拿到配置、触发战斗
        {
            Debug.Log($"玩家进入战斗区域（{hexCell.coord.x}, {hexCell.coord.y}）");
            StartHexCellBattle(hexCell);
            gameCoreModel.isBattling = true;
        }
    }

    private void OnEnemyDefeated(EnemyDefeatedEvent evt)
    {
        gameEntityModel.RemoveActiveEnemy(evt.hexCell, evt.enemy);
    }

    #endregion


    #region 战斗流程
    private void StartHexCellBattle(HexCell hexCell)
    {
        // 1.设置为战斗状态
        gameEntityModel.SetHexCellBattleState(hexCell, HexCellState.InProgress);
        // 2.计算当前区域的怪物配置
        var config = CalculateRegionMonsterConfig(hexCell);
        // 3.触发战斗事件
        StartGenerateMonsterFlow(hexCell);

    }

    // 启动战斗流程
    private void StartGenerateMonsterFlow(HexCell hexCell)
    {
        Debug.Log($"开始区域（{hexCell.coord.x}, {hexCell.coord.y})的战斗流程");
        var config = CalculateRegionMonsterConfig(hexCell);
        var battleState = new BattleFlowState(config);
        activeBattles[hexCell] = battleState;

        // 启动协程管理战斗流程
        battleState.battleCoroutine = 
            gameCoreModel.GameCoreController.StartCoroutine(BattleFlowCoroutine(hexCell, battleState));
    }


    // 战斗流程协程
    private IEnumerator BattleFlowCoroutine(HexCell hexCell, BattleFlowState battleState)
    {
        hexCell.SealRoadEdges();
        // 等待3秒再开始战斗
        yield return new WaitForSeconds(3f);

        OnBattleStart(hexCell, battleState);

        // 配置这个区域内可以生成的怪物
        List<GameObject> availableEnemyPrefabs = new();
        foreach (int i in battleState.config.availableMonsterQuality)
        {
            foreach (GameObject enemyPrefab in gameEntityModel.EnemiesInQualityDict[i])
            {
                availableEnemyPrefabs.Add(enemyPrefab);
            }
        }

        // 按照波次，生成当前波怪物
        for (int wave = 0; wave < battleState.config.waveCount; wave++)
        {
            Debug.Log($"开始第{wave + 1}波怪物生成，当前总敌人数量：{battleState.config.totalEnemies}");
            battleState.currentWave = wave;

           yield return gameCoreModel.GameCoreController.StartCoroutine(SpawnWaveEnemiesCoroutine(hexCell, battleState, wave, availableEnemyPrefabs));

            yield return WaitForWaveComplete(hexCell);

            // 如果不是最后一波，等待2秒
            if (wave < battleState.config.waveCount - 1)
            {
                yield return new WaitForSeconds(2f);
            }
        }

        OnBattleComplete(hexCell);
    }


    // 生成当前波怪物,每次随机选择一个可用的Prefab生成、赋数值
    private IEnumerator SpawnWaveEnemiesCoroutine(HexCell hexCell, BattleFlowState battleState, int waveIndex, List<GameObject> availableEnemyPrefabs)
    {
        MonsterRegionConfig config = battleState.config;
        int enemyCount = battleState.enemiesPerWave[waveIndex];

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject randomEnemy = availableEnemyPrefabs[UnityEngine.Random.Range(0, availableEnemyPrefabs.Count)];
            GameObject enemy = CreateEnemy(hexCell, battleState.config, randomEnemy);
            gameEntityModel.AddActiveEnemy(hexCell, enemy);
            float randomDelay = UnityEngine.Random.Range(0.1f, 0.2f);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    // 在EntityGroup周围随机生成怪物
    private GameObject CreateEnemy(HexCell hexCell, MonsterRegionConfig config, GameObject enemyPrefab)
    {
        var entityGroups = gameEntityModel.GetEntityGroupsForHexCell(hexCell);
        // 随机选择一个EntityGroup作为生成位置
        var randomGroup = entityGroups[UnityEngine.Random.Range(0, entityGroups.Count)];

        // 根据配置，生成怪物实例、修改数值属性
        GameObject enemyInstance = randomGroup.SummonGameObjectFacingHexCellCenter(hexCell, enemyPrefab);
        ConfigureEnemy(enemyInstance, hexCell, config);
        Debug.Log($"在EntityGroup中成功创建敌人: {enemyInstance.name}");

        return enemyInstance;
    }

    // 配置怪物实例的属性和监听器
    private void ConfigureEnemy(GameObject enemyInstance, HexCell hexCell, MonsterRegionConfig config)
    {
        var deathNotifier = enemyInstance.GetComponent<EnemyDeathNotifier>();
        if (deathNotifier == null)
        {
            deathNotifier = enemyInstance.AddComponent<EnemyDeathNotifier>();
        }
        deathNotifier.belongToHexCell = hexCell;

        // 根据enemy基础数值，计算血量、伤害、颜色、元素等属性
        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        enemy.SetVisualColor(config.monsterColor);
        ConfigureEnemyAttribute(enemy, config.monsterLevel);
    }

    // 等待当前波完成
    private IEnumerator WaitForWaveComplete(HexCell hexCell)
    {
        while (gameEntityModel.GetActiveEnemyCount(hexCell) > 0)
        {
            yield return new WaitForSeconds(1.5f);
        }

        // 发送波次完成事件
        var battleState = activeBattles[hexCell];
        this.SendEvent(new WaveCompletedEvent
        {
            hexCell = hexCell,
            currentWave = battleState.currentWave,
            totalWaves = battleState.config.waveCount
        });
    }

    // 战斗开始处理
    private void OnBattleStart(HexCell hexCell, BattleFlowState battleState)
    {
        Debug.Log($"战斗流程开始 - 总敌人数: {battleState.config.totalEnemies}, 波次数: {battleState.config.waveCount}");

        gameEntityModel.SetHexCellBattleState(hexCell, HexCellState.InProgress);
        gameCoreModel.isBattling = true;
    }

    // 战斗完成处理
    private void OnBattleComplete(HexCell hexCell)
    {
        Debug.Log($"区域（{hexCell.coord.x}, {hexCell.coord.y}）的战斗完成");
        gameEntityModel.SetHexCellBattleState(hexCell, HexCellState.AllCompleted);
        gameCoreModel.RegionCompletedCountCurrentLevel.Value++;

        // 清理战斗状态
        activeBattles.Remove(hexCell);

        // 如果最后还有Boss战！那就锁定状态了，击败Boss通关
        if (hexCell.isEndLocation)
        {
            Debug.Log("Boss区域战斗完成，生成Boss！");
            SpawnBoss(hexCell);
            return;
        }

        hexCell.OpenRoadEdges();

        // 检查是否还有其他战斗
        if (activeBattles.Count == 0)
        {
            gameCoreModel.isBattling = false;
        }

        this.SendEvent(new OnBattleRegionClearedEvent { hexCell = hexCell });
    }


    #endregion

    #region 辅助方法

    // 获取并且配置怪物信息。怪物总数=基础数量 + 当前难度下的区域完成数
    private MonsterRegionConfig CalculateRegionMonsterConfig(HexCell hexCell)
    {
        int currentDifficulty = gameCoreModel.DifficultyLevel.Value;
        // 使用AnimationCurve.Evaluate方法获取敌人数量
        int baseEnemyNum = Mathf.RoundToInt(battleDifficultyConfig.enemyNumByDifficultyCurve.Evaluate(currentDifficulty));
        int totalEnemyNum = baseEnemyNum + gameCoreModel.RegionCompletedCountCurrentLevel.Value;
        int maxEnemyNumPerWave = Mathf.RoundToInt(battleDifficultyConfig.enemyMaxNumPerWaveByDifficultyCurve.Evaluate(currentDifficulty));
        int waveCount = Mathf.FloorToInt((float)totalEnemyNum / maxEnemyNumPerWave);
        
        // 如果敌人小于一波最大数量，则至少生成一波
        if (waveCount == 0) waveCount++;

        Debug.Log($"战斗配置计算 - 难度等级: {currentDifficulty}, 基础敌人数: {baseEnemyNum}, 总敌人数: {totalEnemyNum}, 每波最大数: {maxEnemyNumPerWave}, 波次数: {waveCount}");

        // 怪物等级暂定和难度等级相同
        int monsterLevel = currentDifficulty;

        Color monsterColor = hexCell.HexRealm.GetRealmBiome().Color;
        float carryStatusRate = battleDifficultyConfig.enemyCarryStatusRateByDifficultyCurve.Evaluate(currentDifficulty);

        // 根据当前游戏难度，获取可用怪物品质范围
        List<int> availableMonsterQuality = new List<int>();
        Vector2Int quantityRange = battleDifficultyConfig.qualityConfigsByDifficulty[currentDifficulty - 1];
        for (int i = quantityRange.x; i <= quantityRange.y; i++)
        {
            availableMonsterQuality.Add(i);
        }

        return new MonsterRegionConfig
        {
            totalEnemies = totalEnemyNum,
            maxPerWave = maxEnemyNumPerWave,
            waveCount = waveCount,
            monsterLevel = monsterLevel,
            monsterColor = monsterColor,
            carryStatusRate = carryStatusRate,
            availableMonsterQuality = availableMonsterQuality
        };
    }

    // 配置敌人属性计算曲线，简单定为每一级1.3倍难度递增
    private void ConfigureEnemyAttribute(Enemy enemy, int difficultyLevel)
    {
        float difficultyMultiplier = Mathf.Pow(1.3f, difficultyLevel - 1);

        int originalHealth = enemy.enemyData.enemyBaseHealth;
        int originalAttack = enemy.enemyData.enemyBaseAttack;

        int newHealth = Mathf.RoundToInt(originalHealth * difficultyMultiplier);
        int newAttack = Mathf.RoundToInt(originalAttack * difficultyMultiplier);

        enemy.MaxHealth = newHealth;
        enemy.CurrentHealth = newHealth;
        enemy.CurrentAttack = newAttack;
        Debug.Log("AIDirector更新Enemy的属性");
    }

    private void SpawnBoss(HexCell hexCell)
    {
        GameObject bossPrefab = storage.GetBossEnemyPrefab();
        Vector3 spawnPos = GOExtensions.GetGroundPosition(hexCell.transform.position);
        GameObject boss = GameObject.Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        this.GetSystem<AudioSystem>().FadeOutAndChangeMusic(storage.GetMusicBarrenRegion(), 0.5f);
    }
    #endregion

    #region 外部可以用

    public void SummonEnemiesAtHexCell(HexCell hexCell, int enemyCount)
    {
        if (hexCell == null || enemyCount <= 0) return;

        var config = CalculateRegionMonsterConfig(hexCell);

        List<GameObject> availableEnemyPrefabs = GetAvailableEnemyPrefabs(config.availableMonsterQuality);

        if (availableEnemyPrefabs.Count == 0)
        {
            Debug.LogWarning("没有可用的敌人预制体进行召唤");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject randomEnemy = availableEnemyPrefabs[UnityEngine.Random.Range(0, availableEnemyPrefabs.Count)];
            GameObject enemy = CreateEnemy(hexCell, config, randomEnemy);
            gameEntityModel.AddActiveEnemy(hexCell, enemy);
        }
    }

    private List<GameObject> GetAvailableEnemyPrefabs(List<int> availableQualities)
    {
        List<GameObject> availableEnemyPrefabs = new List<GameObject>();

        foreach (int quality in availableQualities)
        {
            if (gameEntityModel.EnemiesInQualityDict.ContainsKey(quality))
            {
                availableEnemyPrefabs.AddRange(gameEntityModel.EnemiesInQualityDict[quality]);
            }
        }

        return availableEnemyPrefabs;
    }


    #endregion
}

// 根据当前游戏状态，进行每一波怪物的配置
public class MonsterRegionConfig
{
    public int totalEnemies;        // 总敌人数量
    public int maxPerWave;          // 每波最大数量
    public int waveCount;           // 波次数量
    public int monsterLevel;        // 怪物属性等级
    public Color monsterColor;      // 怪物颜色
    public float carryStatusRate;   // 携带状态概率
    public List<int> availableMonsterQuality; // 可用怪物品质范围
}