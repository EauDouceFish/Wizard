using QFramework;
using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 一个游戏内Entity模型，管理Entity和EntityGroup的归属数据。
/// </summary>
public class GameEntityModel : AbstractModel, ICanRegisterEvent, ICanGetModel
{
    // 每个HexCell包含的EntityGroup列表
    private Dictionary<HexCell, List<EntityGroup>> hexCellContainingEntityGroups;

    // 每个HexCell的战斗状态
    private Dictionary<HexCell, HexCellState> hexCellBattleStates;

    // 每个HexCell当前活跃的敌人
    private Dictionary<HexCell, List<GameObject>> activeEnemies;

    private Storage storage;

    private GameObject[] allEnemyPrefabs;

    public Dictionary<int, List<GameObject>> EnemiesInQualityDict;

    protected override void OnInit()
    {
        storage = this.GetUtility<Storage>();   
        hexCellContainingEntityGroups = new Dictionary<HexCell, List<EntityGroup>>();
        hexCellBattleStates = new Dictionary<HexCell, HexCellState>();
        activeEnemies = new Dictionary<HexCell, List<GameObject>>();
        
        allEnemyPrefabs = storage.GetAllEnemyPrefabs();
        EnemiesInQualityDict = SortEnemyPrefabsByQuality();

        this.RegisterEvent<MapGenerationCompletedEvent>(InitGameEntityModel)
            .UnRegisterWhenCurrentSceneUnloaded();
    }

    private void InitGameEntityModel(MapGenerationCompletedEvent evt)
    {
        MapModel mapModel = this.GetModel<MapModel>();
        foreach (HexCell hexCell in mapModel.HexGrid.allHexCellCoordDict.Values)
        {
            hexCellBattleStates[hexCell] = HexCellState.NoDiscover;
        }
    }

    #region 外部Get/Set方法

    /// <summary>
    /// 将EntityGroup绑定添加到指定的HexCell中
    /// </summary>
    public void AddEntityGroupToHexCell(HexCell hexCell, EntityGroup entityGroup)
    {
        if (!hexCellContainingEntityGroups.ContainsKey(hexCell))
        {
            hexCellContainingEntityGroups[hexCell] = new List<EntityGroup>();
        }

        hexCellContainingEntityGroups[hexCell].Add(entityGroup);
    }

    /// <summary>
    /// 获取HexCell的战斗状态
    /// </summary>
    public HexCellState GetHexCellBattleState(HexCell hexCell)
    {
        return hexCellBattleStates.TryGetValue(hexCell, out var state) ? state : HexCellState.NoDiscover;
    }

    public void SetHexCellBattleState(HexCell hexCell, HexCellState state)
    {
        this.SendEvent<OnRegionStateChangedEvent>(new OnRegionStateChangedEvent
        {
            hexCell = hexCell,
            newState = state
        });
        hexCellBattleStates[hexCell] = state;
    }

    public List<EntityGroup> GetEntityGroupsForHexCell(HexCell hexCell)
    {
        return hexCellContainingEntityGroups.TryGetValue(hexCell, out var groups) ? groups : new List<EntityGroup>();
    }


    // 添加活跃敌人管理方法
    public void AddActiveEnemy(HexCell hexCell, GameObject enemy)
    {
        if (!activeEnemies.ContainsKey(hexCell))
        {
            activeEnemies[hexCell] = new List<GameObject>();
        }
        activeEnemies[hexCell].Add(enemy);
    }

    public void RemoveActiveEnemy(HexCell hexCell, GameObject enemy)
    {
        if (activeEnemies.TryGetValue(hexCell, out var enemies))
        {
            enemies.Remove(enemy);
        }
    }

    public int GetActiveEnemyCount(HexCell hexCell)
    {
        return activeEnemies.TryGetValue(hexCell, out var enemies) ? enemies.Count : 0;
    }

    public List<GameObject> GetActiveEnemies(HexCell hexCell)
    {
        return activeEnemies.TryGetValue(hexCell, out var enemies) ? enemies : new List<GameObject>();
    }

    /// <summary>
    /// 把敌人按照按品质分组
    /// </summary>
    private Dictionary<int, List<GameObject>> SortEnemyPrefabsByQuality()
    {
        Dictionary<int, List<GameObject>> enemyPrefabsByQuality = new Dictionary<int, List<GameObject>>();

        foreach (GameObject enemyPrefab in allEnemyPrefabs)
        {
            // 从 prefab 中获取 EnemyEntityData 组件
            Enemy enemy = enemyPrefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                int quality = enemy.enemyData.enemyQuality;

                if (!enemyPrefabsByQuality.ContainsKey(quality))
                {
                    enemyPrefabsByQuality[quality] = new List<GameObject>();
                }

                enemyPrefabsByQuality[quality].Add(enemyPrefab);
            }
        }

        return enemyPrefabsByQuality;
    }

    #endregion
}

/// <summary>
/// 战斗状态枚举
/// </summary>
public enum HexCellState
{
    NoDiscover,     // 没有进入
    InProgress,     // 进入-战斗中
    AllCompleted    // 清理完成
}