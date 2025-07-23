using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class DecorationFillStep : IMapGenerationStep
{
    Storage storage;
    DecorationDistributionConfig fillConfig;
    GameObject decorationGroup;
    public void Execute(MapModel mapModel)
    {
        storage = mapModel.GetUtility<Storage>();
        fillConfig = storage.GetDecorationDistributionConfig();

        CreateDecotarionGroup();

        foreach (HexCell hexCell in mapModel.HexGrid.allHexCellCoordDict.Values)
        {
            if (!hexCell.isOccupied) continue;
            FillHexCellDecorations(hexCell);
        }
    }


    private void CreateDecotarionGroup()
    {
        decorationGroup = GameObject.Find("DecorationGroups");
        if (decorationGroup == null)
        {
            decorationGroup = new GameObject("DecorationGroups");
        }
        decorationGroup.transform.position = Vector3.zero;
    }


    /// <summary>
    /// ���װ����ϰ�������һ��Edge����Road���ͻ����Edge���ڵ�HexCell���װ���
    /// װ���߼���һ��HexCellͨ����4-5��Edge����Road��
    /// ������Щ!isRoad��Edge�������ǰ���HexCellΪ��λ���֣�ÿ��HexCell��Ϊ0 ~1��ï���������1 ~n��ϡ�����򡣣��������ã�
    /// ��Щ���������������Щ�ߣ��ڱ������ѡһ����λ��vector:start->end* ramdom(0, 1)������������HexCell���ĵķ����������������ƫ�ƣ����װ���
    /// װ����ʱ��Ϊ2���裺
    /// װ��һ��װ���ͨ��Ϊ��ľ��������ʯ�ȴ���װ���
    /// װ�����װ���ͨ��ΪСʯ�顢Log�����ݵ�С��װ���
    /// ÿ�������ӵ�е�1��2��װ���������������á�
    /// </summary>
    public void FillHexCellDecorations(HexCell hexCell)
    {
        // 1.�ռ����п��������װ����ı�
        List<HexCellEdge> filledEdges = new();
        foreach (HexCellEdge edge in hexCell.edges)
        {
            // ���һ�����Ѿ�������ϰ���Ϳ����������װ������
            if (edge.GetIsFilled())
            {
                filledEdges.Add(edge);
            }
        }

        if (filledEdges.Count == 0)
        {
            Debug.LogWarning("�տ�����û�п��ñߣ��������������������������");
            return;
        }

        // 2.�������ã����ѡ��ï�������ϡ������������������װ�������ռ�����
        int denseRegionCount = Random.Range(fillConfig.minDenseRegionCount, fillConfig.maxDenseRegionCount + 1);
        int sparseRegionCount = Random.Range(fillConfig.minSparseRegionCount, fillConfig.maxSparseRegionCount + 1);

        List<DecorationRegion> regions = new();
        for (int i = 0; i < denseRegionCount; i++)
        {
            regions.Add(new DecorationRegion(RegionType.Dense, hexCell));
        }
        for (int i = 0; i < sparseRegionCount; i++)
        {
            regions.Add(new DecorationRegion(RegionType.Sparse, hexCell));
        }

        // 3.��װ����������������
        foreach (DecorationRegion region in regions)
        {
            if (filledEdges.Count > 0)
            {
                int randomIndex = Random.Range(0, filledEdges.Count);
                region.AssignedEdge = filledEdges[randomIndex];
            }
        }

        // 4.���װ����
        foreach (DecorationRegion region in regions)
        {
            if (region.AssignedEdge != null)
            {
                FillRegionDecorations(region, hexCell.HexRealm.GetRealmBiome());
            }
        }
    }

    /// <summary>
    /// Ϊ�ض��������װ����
    /// </summary>
    private void FillRegionDecorations(DecorationRegion region, Biome biome)
    {
        // ���һ��װ����
        FillDecoration(region, biome, DecorationLevel.Level1);

        // ������װ����
        FillDecoration(region, biome, DecorationLevel.Level2);
    }

    /// <summary>
    /// ͳһ��װ������䷽��
    /// </summary>
    private void FillDecoration(DecorationRegion region, Biome biome, DecorationLevel level)
    {

        // ��ȡװ����ģ��
        GameObject[] decorationModels = GetDecorationModels(biome, level);
        if (decorationModels.Length == 0) return;

        // ��ȡװ������������
        int decorationCount = GetDecorationCountRangeByRegion(region.RegionType, level);
        // ��ȡƫ�ƾ��뷶Χ
        (float minOffset, float maxOffset) = GetOffsetRangeByLevel(level);

        for (int i = 0; i < decorationCount; i++)
        {
            // �ڱ������ѡ��һ���㣬������ƫ��
            var (startPoint, endPoint) = region.AssignedEdge.GetEdgePoints();
            Vector3 edgeVector = endPoint - startPoint;
            float randomParam = Random.Range(0f, 1f);
            Vector3 edgePosition = startPoint + edgeVector * randomParam;
            Vector3 directionToCenter = (region.HexCell.transform.position - edgePosition).normalized;

            // ���ƫ���������ϵ���Model��ƫ��
            float offsetDistance = Random.Range(minOffset, maxOffset);

            Vector3 decorationPosition = edgePosition + directionToCenter * offsetDistance;
            decorationPosition = GOExtensions.GetGroundPosition(decorationPosition);

            // ���ѡ��ģ��
            GameObject selectedModel = decorationModels[Random.Range(0, decorationModels.Length)];
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Vector3 offset = selectedModel.GetModelGeometryOffsetPos();
            // ����װ����ʵ��
            GameObject decorationInstance = Object.Instantiate(selectedModel, decorationPosition + offset, randomRotation);
            decorationInstance.transform.SetParent(decorationGroup.transform, false);
        }
    }

    /// <summary>
    /// �����������ͺ�װ����ȼ���ȡװ��������
    /// </summary>
    private int GetDecorationCountRangeByRegion(RegionType regionType, DecorationLevel level)
    {
        return (regionType, level) switch
        {
            (RegionType.Dense, DecorationLevel.Level1) =>
                Random.Range(fillConfig.minLevel1DecorationCount_Dense, fillConfig.maxLevel1DecorationCount_Dense + 1),
            (RegionType.Dense, DecorationLevel.Level2) =>
                Random.Range(fillConfig.minLevel2DecorationCount_Dense, fillConfig.maxLevel2DecorationCount_Dense + 1),
            (RegionType.Sparse, DecorationLevel.Level1) =>
                Random.Range(fillConfig.minLevel1DecorationCount_Sparse, fillConfig.maxLevel1DecorationCount_Sparse + 1),
            (RegionType.Sparse, DecorationLevel.Level2) =>
                Random.Range(fillConfig.minLevel2DecorationCount_Sparse, fillConfig.maxLevel2DecorationCount_Sparse + 1),
            _ => 0
        };
    }

    /// <summary>
    /// ��������Ⱥ���װ����ȼ���ȡװ����ģ��
    /// </summary>
    private GameObject[] GetDecorationModels(Biome biome, DecorationLevel level)
    {
        GameObject[] models;
        switch(level)
        {
            case DecorationLevel.Level1:
                models = storage.GetLevel1DecorationModels(biome);
                break;  
            case DecorationLevel.Level2:
                models = storage.GetLevel2DecorationModels(biome);
                break;
            default:
                Debug.LogWarning($"Ⱥϵ{biome}��û�����{level}����DecorationModel");
                return null;
        };
        return models;
    }

    /// <summary>
    /// ����װ����ȼ���ȡƫ�ƾ��뷶Χ
    /// </summary>
    private (float min, float max) GetOffsetRangeByLevel(DecorationLevel level)
    {
        return level switch
        {
            DecorationLevel.Level1 => (fillConfig.minOffsetLevel1, fillConfig.maxOffsetLevel1),
            DecorationLevel.Level2 => (fillConfig.minOffsetLevel2, fillConfig.maxOffsetLevel2),
            _ => (0f, 0f)
        };
    }
}
public class DecorationRegion
{
    public RegionType RegionType { get; private set; }
    public HexCell HexCell { get; private set; }
    public HexCellEdge AssignedEdge { get; set; }

    public DecorationRegion(RegionType regionType, HexCell hexCell)
    {
        RegionType = regionType;
        HexCell = hexCell;
        AssignedEdge = default;
    }
}
public enum RegionType
{
    Dense,
    Sparse,
    Custom
}

public enum DecorationLevel
{
    Level1,
    Level2
}