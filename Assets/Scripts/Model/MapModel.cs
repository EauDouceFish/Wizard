using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��Ϸ��ͼ����Model
/// </summary>
public class MapModel : AbstractModel
{
    public int MapWidth { get; private set; }                            // ��ͼ��
    public int MapHeight { get; private set; }                           // ��ͼ��
    public int MapTargetBiomeCount { get; private set; }                 // ��ͼĿ��Ⱥϵ����
    public int BiomeCount => biomes.Count;                               // ʵ��Ⱥϵ����
    public float HexOuterRadius { get; private set; }                    // ��ͼʹ�õ�����������Բ�뾶���߳���
    public float HexInnerRadius => HexOuterRadius * 0.866025404f;        // ��ͼʹ�õ��������ڽ�Բ�뾶
    public float ScaleParam => HexOuterRadius / 40.0f;                  // ��ͼ���Ų���������ģ������ʹ��

    // ��ǰ��ͼ��С����
    public MapSize CurrentMapSize { get; private set; }

    // ��ǰ��ͼӵ�е�Ⱥϵ�б�
    public List<Biome> biomes { get; private set; } = new List<Biome>();

    // ��ͼ����Ҫ��Ⱥϵ��������
    public List<BiomeSO> BiomeConfigData { get; private set; } = new List<BiomeSO>();

    // ˫�˶��У����ڴ洢��·���ҿ�����ǰ�����
    public List<HexCell> MainPath { get; private set; } = new List<HexCell>();

    // ��ǰ��ͼ���������������ͼ
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
    /// ���δ���õ�ͼ��С��ģ���򲻽��������һ������
    /// </summary>
    /// <returns></returns>
    public bool GetIsMapSizeValid()
    {
        return CurrentMapSize != MapSize.Default;
    }

    #region �����߼�������Controller/Command��

    public void SetMapWidth(int width)
    {
        MapWidth = width;
    }

    public void SetMapHeight(int height)
    {
        MapHeight = height;
    }

    /// <summary>
    /// ���õ�ǰ��ͼ��С��ģ����
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
    /// ���Boime����Ⱥϵ���ṩ��HexGrid����
    /// </summary>
    /// <param name="biome"></param>
    internal void AddBiome(Biome biome)
    {
        biomes.Add(biome);
    }

    #endregion
}