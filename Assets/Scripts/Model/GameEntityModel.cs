using QFramework;
using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// һ����Ϸ��Entityģ�ͣ�����Entity��EntityGroup�Ĺ������ݡ�
/// </summary>
public class GameEntityModel : AbstractModel, ICanRegisterEvent, ICanGetModel
{
    // ÿ��HexCell������EntityGroup�б�
    private Dictionary<HexCell, List<EntityGroup>> hexCellContainingEntityGroups;

    // ÿ��HexCell��ս��״̬
    private Dictionary<HexCell, HexCellState> hexCellBattleStates;

    // ÿ��HexCell��ǰ��Ծ�ĵ���
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

    #region �ⲿGet/Set����

    /// <summary>
    /// ��EntityGroup����ӵ�ָ����HexCell��
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
    /// ��ȡHexCell��ս��״̬
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


    // ��ӻ�Ծ���˹�����
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
    /// �ѵ��˰��հ�Ʒ�ʷ���
    /// </summary>
    private Dictionary<int, List<GameObject>> SortEnemyPrefabsByQuality()
    {
        Dictionary<int, List<GameObject>> enemyPrefabsByQuality = new Dictionary<int, List<GameObject>>();

        foreach (GameObject enemyPrefab in allEnemyPrefabs)
        {
            // �� prefab �л�ȡ EnemyEntityData ���
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
/// ս��״̬ö��
/// </summary>
public enum HexCellState
{
    NoDiscover,     // û�н���
    InProgress,     // ����-ս����
    AllCompleted    // �������
}