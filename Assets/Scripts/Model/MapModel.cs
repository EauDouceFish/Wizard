using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 游戏地图数据Model
/// </summary>
public class MapModel : AbstractModel
{
    public int MapWidth { get; private set; }                            // 地图宽
    public int MapHeight { get; private set; }                           // 地图高
    public int MapTargetBiomeCount { get; private set; }                 // 地图目标群系数量
    public int BiomeCount => biomes.Count;                               // 实际群系数量
    public float HexOuterRadius { get; private set; }                    // 地图使用的六边形外切圆半径（边长）
    public float HexInnerRadius => HexOuterRadius * 0.866025404f;        // 地图使用的六边形内接圆半径
    public float ScaleParam => HexOuterRadius / 40.0f;                  // 地图缩放参数，地形模型缩放使用

    // 当前地图大小类型
    public MapSize CurrentMapSize { get; private set; }

    // 当前地图拥有的群系列表
    public List<Biome> biomes { get; private set; } = new List<Biome>();

    // 地图所需要的群系配置数据
    public List<BiomeSO> BiomeConfigData { get; private set; } = new List<BiomeSO>();

    // 双端队列，便于存储主路径且可以在前后操作
    public List<HexCell> MainPath { get; private set; } = new List<HexCell>();

    // 当前地图的生成器，网格地图
    public HexGrid HexGrid { get; private set; }

    protected override void OnInit()
    {
        if (MapConfigurationManager.HasValidMapSize())
        {
            CurrentMapSize = MapConfigurationManager.GetSelectedMapSize();
        }
        else
        {
            CurrentMapSize = MapSize.Default;
        }
    }

    /// <summary>
    /// 如果未设置地图大小规模，则不建议进行下一步操作
    /// </summary>
    /// <returns></returns>
    public bool GetIsMapSizeValid()
    {
        return CurrentMapSize != MapSize.Default;
    }

    #region 简易逻辑方法（Controller/Command）

    public void SetMapWidth(int width)
    {
        MapWidth = width;
    }

    public void SetMapHeight(int height)
    {
        MapHeight = height;
    }

    /// <summary>
    /// 设置当前地图大小规模数据
    /// </summary>
    /// <param name="mapSize"></param>
    public void SetCurrentMapSize(MapSize mapSize)
    {
        CurrentMapSize = mapSize;
        MapConfigurationManager.SetSelectedMapSize(mapSize);
    }

    public void SetMapTargetBiomeCount(int count)
    {
        MapTargetBiomeCount = count;
    }

    public void SetHexOuterRadius(float radius)
    {
        HexOuterRadius = radius;
    }

    public void SetHexGrid(HexGrid hexGrid)
    {
        HexGrid = hexGrid;
    }

    public void SetBiomeConfigData(List<BiomeSO> biomeDataList)
    {
        BiomeConfigData = biomeDataList;
    }

    public void SetBiomeConfigData(BiomeSO[] biomeDataList)
    {
        BiomeConfigData = new List<BiomeSO>(biomeDataList);
    }
    /// <summary>
    /// 添加Boime进入群系，提供给HexGrid调用
    /// </summary>
    /// <param name="biome"></param>
    internal void AddBiome(Biome biome)
    {
        biomes.Add(biome);
    }

    #endregion
}