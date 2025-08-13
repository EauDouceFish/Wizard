using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

/// <summary>
/// 生成六边形网格地图的控制器
/// </summary>
public class MapGenerationController : MonoBehaviour, IController, ICanSendCommand
{
    [SerializeField] HexMetricsConfigData hexMetricsConfigData;
    [SerializeField] MapConfigData mapConfigData; //这个数据只是参考，运行时从MapModel读取。Model的赋值此前完成
    MapGenerationSystem mapGenerationSystem;
    MapModel mapModel;

    public bool shouldUseDefault = true;

    // 自定义地图规格
    public int m_width;
    public int m_height;
    public int m_biomeCount;

    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();
        InitMapModel(); 
    }

    private void Start()
    {
        mapGenerationSystem.ExecuteGenerationPipeline();
    }

    private void InitMapModel()
    {
        if (shouldUseDefault)
        {
            mapModel.SetMapWidth(GetMapWidthDataBySize(mapModel.CurrentMapSize));
            mapModel.SetMapHeight(GetMapHeightDataBySize(mapModel.CurrentMapSize));
            mapModel.SetMapTargetBiomeCount(GetMapBiomeCountBySize(mapModel.CurrentMapSize));
        }
        else
        {
            mapModel.SetMapWidth(m_width);
            mapModel.SetMapHeight(m_height);
            mapModel.SetMapTargetBiomeCount(m_biomeCount);
        }

        mapModel.SetHexOuterRadius(hexMetricsConfigData.OuterRadius);
    }

    #region 辅助方法

    private int GetMapWidthDataBySize(MapSize size)
    {
        int mapWidth = 0;
        switch (size)
        {
            case MapSize.Default:
                Debug.LogWarning("地图规模未设置，默认宽度为Mini版本");
                mapWidth = mapConfigData.MiniMapWidth;
                break;
            case MapSize.Mini:
                mapWidth = mapConfigData.MiniMapWidth;
                break;
            case MapSize.Small:
                mapWidth = mapConfigData.SmallMapWidth;
                break;
            case MapSize.Medium:
                mapWidth = mapConfigData.MediumMapWidth;
                break;
            case MapSize.Large:
                mapWidth = mapConfigData.LargeMapWidth;
                break;
        };
        Debug.Log($"地图宽度设置为{size}");
        return mapWidth;
    }

    private int GetMapHeightDataBySize(MapSize size)
    {
        int mapHeight = 0;
        switch (size)
        {
            case MapSize.Default:
                Debug.LogWarning("地图规模未设置，默认高度为Mini版本");
                mapHeight = mapConfigData.MiniMapHeight;
                break;
            case MapSize.Mini:
                mapHeight = mapConfigData.MiniMapHeight;
                break;
            case MapSize.Small:
                mapHeight = mapConfigData.SmallMapHeight;
                break;
            case MapSize.Medium:
                mapHeight = mapConfigData.MediumMapHeight;
                break;
            case MapSize.Large:
                mapHeight = mapConfigData.LargeMapHeight;
                break;
        };
        Debug.Log($"地图高度设置为{size}");
        return mapHeight;
    }

    /// <summary>
    /// 根据地图尺寸返回可支持的群系数量
    /// </summary>
    private int GetMapBiomeCountBySize(MapSize size)
    {
        int biomeCount = 0;
        switch (size)
        {
            case MapSize.Default:
                Debug.LogWarning("地图规模未设置，默认群系数量为Mini版本");
                biomeCount = mapConfigData.MaxBoimeSupportMini;
                break;
            case MapSize.Mini:
                biomeCount = mapConfigData.MaxBoimeSupportMini;
                break;
            case MapSize.Small:
                biomeCount = mapConfigData.MaxBoimeSupportSmall;
                break;
            case MapSize.Medium:
                biomeCount = mapConfigData.MaxBoimeSupportMedium;
                break;
            case MapSize.Large:
                biomeCount = mapConfigData.MaxBoimeSupportLarge;
                break;
        };
        Debug.Log($"地图群系数量设置为{biomeCount}（地图规模：{size}）");
        return biomeCount;
    }

    #endregion

    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
