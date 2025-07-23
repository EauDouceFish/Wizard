using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using System;

/// <summary>
/// 六边形网格地图大类，包含地图创建、Mesh简化、寻路功能等
/// </summary>
public class HexGrid : MonoBehaviour, IController
{
    private MapGenerationSystem mapGenerationSystem;
    private MapModel mapModel;
    private GameObject hexCellsCollector;
    private HexCellPathFinder hexCellPathFinder;                    // 六边形Cell寻路器
    private HexMesh hexMesh;
    private EdgeFillConfig edgeFillConfig;                          // 边缘填充障碍的配置

    [SerializeField] private HexCell cellPrefab;                    // 六边形Cell预制体

    public Dictionary<Pos2D, HexCell> allHexCellCoordDict = new();  // 存储了所有HexCell，以及所有坐标映射

    private List<HexRealm> hexRealms = new List<HexRealm>();        // 六边形领域列表

    private HexCell[] cells;
    private float outerRadius;
    private float innerRadius;
    
    #region 生命周期

    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();
        var storage = this.GetUtility<Storage>();

        // 存储六边形网格地图数据、群系配置数据
        mapModel.SetHexGrid(this);
        mapModel.SetBiomeConfigData(storage.GetAllBiomeSO());

        // 所有HexGrid内网格共享一份数据
        hexMesh = this.GetOrAddComponent<HexMesh>();
        InitHexCellPathFinder();
    }

    private void Start()
    {
        // 缓存变量
        outerRadius = mapModel.HexOuterRadius;
        innerRadius = mapModel.HexInnerRadius;

        CreateHexCollector();
    }
    #endregion

    #region 外部可以方法

    /// <summary>
    /// 传入要初始化的HexCell数量与大小，之后进行计算并创建六边形网格
    /// </summary>
    public void CreateHexGridXZ(int width = 16, int height = 8)
    {
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCellXZ(x, z, i++);
            }
        }

        hexMesh.Triangulate(cells);
    }

    public HexCell GetCellByCoord(Pos2D coord)
    {
        if (allHexCellCoordDict.TryGetValue(coord, out HexCell cell))
        {
            return cell;
        }
        return null;
    }

    public HexCell GetCellByCoord(int x, int z)
    {
        Pos2D coord = new(x, z);
        if (allHexCellCoordDict.TryGetValue(coord, out HexCell cell))
        {
            return cell;
        }
        return null;
    }

    /// <summary>
    /// 获取随机数量的HexCell
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public HexCell[] GetRandomCells(int count)
    {
        if (allHexCellCoordDict.Count == 0)
        {
            return new HexCell[0];
        }

        int actualCount = Mathf.Min(count, allHexCellCoordDict.Count);

        // 获取所有HexCell等概率挑选
        List<HexCell> allCells = new List<HexCell>(allHexCellCoordDict.Values);

        HexCell[] randomCells = new HexCell[actualCount];

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allCells.Count);
            randomCells[i] = allCells[randomIndex];
            allCells.RemoveAt(randomIndex);
        }

        return randomCells;
    }

    /// <summary>
    /// 在网格地图内创建一个带有群系的Realm
    /// </summary>
    /// <param name="initHexCell"></param>
    /// <param name="biome"></param>
    public void CreateHexRealm(HexCell initHexCell, Biome biome)
    {
        HexRealm hexRealm = new HexRealm(initHexCell, biome);
        hexRealms.Add(hexRealm);
        // 此处后续可修改为事件
        mapModel.AddBiome(biome);
    }

    public List<HexRealm> GetHexRealms()
    {
        return hexRealms;
    }

    public HexCellPathFinder GetHexCellPathFinder()
    {
        return hexCellPathFinder;
    }


    #endregion

    #region 私有逻辑方法

    /// <summary>
    /// 创建六边形Cell的收集器
    /// </summary>
    private void CreateHexCollector()
    {
        if (hexCellsCollector == null)
        {
            hexCellsCollector = new GameObject("HexCellsCollector");
            hexCellsCollector.transform.position = Vector3.zero;
            hexCellsCollector.transform.SetParent(this.transform, false);
        }
    }

    /// <summary>
    /// 创造单个六边形Cell
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="index"></param>
    private void CreateCellXZ(int x, int z, int index)
    {
        Vector3 position = CalculateNewCenterXZ(x, z);
        Pos2D coord = new(x, z);
        HexCell cell = cells[index] = Instantiate(cellPrefab);
        cell.transform.SetParent(hexCellsCollector.transform, false);

        // 初始时不设置网格，稍后通过 HexMesh 来设置
        cell.InitializeXZ(position, coord);
        if (allHexCellCoordDict.ContainsKey(coord))
        {
            Debug.LogWarning($"index: {index}, ({coord.x},{coord.y}) 已经存在");
        }
        else
        allHexCellCoordDict.Add(coord, cell);
    }

    /// <summary>
    /// 以3D空间XZ平面计算的六边形中心
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private Vector3 CalculateNewCenterXZ(int x, int z)
    {
        Vector3 position;
        position.x = x * outerRadius * 1.5f;
        position.y = 0f;
        position.z = z * innerRadius * 2f;

        // 奇数行横向偏移半个外半径，以对其网格
        if (x % 2 == 1)
        {
            position.z -= innerRadius;
        }
        return position;
    }

    private void InitHexCellPathFinder()
    {
        GameObject hexCellPathFinder = new GameObject("HexCellPathFinder");
        hexCellPathFinder.transform.SetParent(transform);
        hexCellPathFinder.transform.localPosition = Vector3.zero;
        this.hexCellPathFinder = hexCellPathFinder.AddComponent<HexCellPathFinder>();
    }

    #endregion

    #region Interfaces

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
