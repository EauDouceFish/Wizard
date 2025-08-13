using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

/// <summary>
/// ���������������ͼ�Ŀ�����
/// </summary>
public class MapGenerationController : MonoBehaviour, IController, ICanSendCommand
{
    [SerializeField] HexMetricsConfigData hexMetricsConfigData;
    [SerializeField] MapConfigData mapConfigData; //�������ֻ�ǲο�������ʱ��MapModel��ȡ��Model�ĸ�ֵ��ǰ���
    MapGenerationSystem mapGenerationSystem;
    MapModel mapModel;

    public bool shouldUseDefault = true;

    // �Զ����ͼ���
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

    #region ��������

    private int GetMapWidthDataBySize(MapSize size)
    {
        int mapWidth = 0;
        switch (size)
        {
            case MapSize.Default:
                Debug.LogWarning("��ͼ��ģδ���ã�Ĭ�Ͽ��ΪMini�汾");
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
        Debug.Log($"��ͼ�������Ϊ{size}");
        return mapWidth;
    }

    private int GetMapHeightDataBySize(MapSize size)
    {
        int mapHeight = 0;
        switch (size)
        {
            case MapSize.Default:
                Debug.LogWarning("��ͼ��ģδ���ã�Ĭ�ϸ߶�ΪMini�汾");
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
        Debug.Log($"��ͼ�߶�����Ϊ{size}");
        return mapHeight;
    }

    /// <summary>
    /// ���ݵ�ͼ�ߴ緵�ؿ�֧�ֵ�Ⱥϵ����
    /// </summary>
    private int GetMapBiomeCountBySize(MapSize size)
    {
        int biomeCount = 0;
        switch (size)
        {
            case MapSize.Default:
                Debug.LogWarning("��ͼ��ģδ���ã�Ĭ��Ⱥϵ����ΪMini�汾");
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
        Debug.Log($"��ͼȺϵ��������Ϊ{biomeCount}����ͼ��ģ��{size}��");
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
