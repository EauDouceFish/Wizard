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
    /// 填充装饰物（障碍物）：如果一个Edge不是Road，就会向该Edge所在的HexCell填充装饰物。
    /// 装填逻辑：一个HexCell通常有4-5个Edge不是Road。
    /// 对于这些!isRoad的Edge，将他们按照HexCell为单位划分，每个HexCell有为0 ~1个茂密区域，随机1 ~n个稀疏区域。（可以配置）
    /// 这些区域会随机分配给这些边，在边上随机选一个点位（vector:start->end* ramdom(0, 1)），让它们往HexCell中心的方向按照配置随机稍做偏移，填充装饰物。
    /// 装填暂时分为2步骤：
    /// 装填一级装饰物，通常为树木、大型岩石等大型装饰物。
    /// 装填二级装饰物，通常为小石块、Log、花草等小型装饰物。
    /// 每个区域可拥有的1、2级装饰物数量可以配置。
    /// </summary>
    public void FillHexCellDecorations(HexCell hexCell)
    {
        // 1.收集所有可向内填充装饰物的边
        List<HexCellEdge> filledEdges = new();
        foreach (HexCellEdge edge in hexCell.edges)
        {
            // 如果一条边已经填充了障碍物，就可以向内填充装饰物了
            if (edge.GetIsFilled())
            {
                filledEdges.Add(edge);
            }
        }

        if (filledEdges.Count == 0)
        {
            Debug.LogWarning("空旷场地没有可用边，后续如果有需求，其他方案处理");
            return;
        }

        // 2.根据配置，随机选择茂密区域和稀疏区域的数量，打包成装饰区域收集起来
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

        // 3.将装饰区域随机分配给边
        foreach (DecorationRegion region in regions)
        {
            if (filledEdges.Count > 0)
            {
                int randomIndex = Random.Range(0, filledEdges.Count);
                region.AssignedEdge = filledEdges[randomIndex];
            }
        }

        // 4.填充装饰物
        foreach (DecorationRegion region in regions)
        {
            if (region.AssignedEdge != null)
            {
                FillRegionDecorations(region, hexCell.HexRealm.GetRealmBiome());
            }
        }
    }

    /// <summary>
    /// 为特定区域填充装饰物
    /// </summary>
    private void FillRegionDecorations(DecorationRegion region, Biome biome)
    {
        // 填充一级装饰物
        FillDecoration(region, biome, DecorationLevel.Level1);

        // 填充二级装饰物
        FillDecoration(region, biome, DecorationLevel.Level2);
    }

    /// <summary>
    /// 统一的装饰物填充方法
    /// </summary>
    private void FillDecoration(DecorationRegion region, Biome biome, DecorationLevel level)
    {

        // 获取装饰物模型
        GameObject[] decorationModels = GetDecorationModels(biome, level);
        if (decorationModels.Length == 0) return;

        // 获取装饰物数量配置
        int decorationCount = GetDecorationCountRangeByRegion(region.RegionType, level);
        // 获取偏移距离范围
        (float minOffset, float maxOffset) = GetOffsetRangeByLevel(level);

        for (int i = 0; i < decorationCount; i++)
        {
            // 在边上随机选择一个点，向中心偏移
            var (startPoint, endPoint) = region.AssignedEdge.GetEdgePoints();
            Vector3 edgeVector = endPoint - startPoint;
            float randomParam = Random.Range(0f, 1f);
            Vector3 edgePosition = startPoint + edgeVector * randomParam;
            Vector3 directionToCenter = (region.HexCell.transform.position - edgePosition).normalized;

            // 随机偏移量、加上地面Model的偏移
            float offsetDistance = Random.Range(minOffset, maxOffset);

            Vector3 decorationPosition = edgePosition + directionToCenter * offsetDistance;
            decorationPosition = GOExtensions.GetGroundPosition(decorationPosition);

            // 随机选择模型
            GameObject selectedModel = decorationModels[Random.Range(0, decorationModels.Length)];
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Vector3 offset = selectedModel.GetModelGeometryOffsetPos();
            // 创建装饰物实例
            GameObject decorationInstance = Object.Instantiate(selectedModel, decorationPosition + offset, randomRotation);
            decorationInstance.transform.SetParent(decorationGroup.transform, false);
        }
    }

    /// <summary>
    /// 根据区域类型和装饰物等级获取装饰物数量
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
    /// 根据生物群落和装饰物等级获取装饰物模型
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
                Debug.LogWarning($"群系{biome}还没有填充{level}级别DecorationModel");
                return null;
        };
        return models;
    }

    /// <summary>
    /// 根据装饰物等级获取偏移距离范围
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