using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 初始化群系起始位置
/// </summary>
public class InitBiomeStep : IMapGenerationStep
{
    public void Execute(MapModel mapModel)
    {
        HexCell[] cells = GetValidHexCells(mapModel);

        if (cells.Length == 0)
        {
            Debug.LogWarning("没有找到有效的HexCell，使用随机格子初始化群系。这可能有关卡错误隐患");
            cells = mapModel.HexGrid.GetRandomCells(mapModel.BiomeConfigData.Count);
        }

        List<BiomeSO> shuffledBiomes = new List<BiomeSO>(mapModel.BiomeConfigData);
        ShuffleBiomeList(shuffledBiomes);

        for (int i = 0; i < mapModel.MapTargetBiomeCount; i++)
        {
            BiomeSO biomeData = shuffledBiomes[i];
            Biome biome = new(biomeData);
            HexCell initCell = cells[i];

            // 初始格子更明显
            MeshRenderer renderer = initCell.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat("_Metallic", 1.0f);
            }
            //Debug.Log($"Initializing Biome: {biome.BiomeName} at position ({initCell.coord.x}, {initCell.coord.y})");

            mapModel.HexGrid.CreateHexRealm(initCell, biome);
        }
    }


    /// <summary>
    /// Fisher-Yates，O（n）洗牌
    /// </summary>
    private void ShuffleBiomeList(List<BiomeSO> biomeList)
    {
        for (int i = biomeList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            BiomeSO temp = biomeList[i];
            biomeList[i] = biomeList[randomIndex];
            biomeList[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 获得Count个合法的HexCell，暂时不做过多限制
    /// </summary>
    /// <param name="mapModel"></param>
    /// <returns></returns>
    private HexCell[] GetValidHexCells(MapModel mapModel)
    {
        int targetCellNum = mapModel.MapTargetBiomeCount;

        var dict = mapModel.HexGrid.allHexCellCoordDict;

        HexCell[] result = new HexCell[targetCellNum];

        if (dict.Count < targetCellNum)
        {
            return new HexCell[0];
        }

        // 获取所有HexCell先随机挑选
        List<HexCell> allCells = new List<HexCell>(dict.Values);

        int loopCount = 0; 
        int selectedCount = 0;
        while (selectedCount < targetCellNum)
        {
            //每次随机选一个，Valid就加入select（第一个就直接加入）
            int randomIndex = Random.Range(0, allCells.Count);
            HexCell candidate = allCells[randomIndex];
            if (selectedCount == 0)
            {
                result[0] = candidate;
                selectedCount++;
                allCells.RemoveAt(randomIndex);
            }
            else
            {
                // 检查是否Valid，公式为 |x2 - x1| *0.5 + |y2 - y1| > 2
                int x1 = result[selectedCount - 1].coord.x;
                int y1 = result[selectedCount - 1].coord.y;
                int x2 = candidate.coord.x;
                int y2 = candidate.coord.y;

                bool distanceValid = (Mathf.Abs(x2 - x1) * 0.5f + Mathf.Abs(y2 - y1)) > 2;
                bool noCenterNearby = true;
                // 检查周围一圈6个格子是否有其他result的元素
                foreach (HexCell otherInitCells in result)
                {
                    if (otherInitCells == null) continue;
                    // 判断是否与candidate相邻，相邻则candidate非法，后续if跳过
                    List<Pos2D> neighborCoords = HexMetrics.GetHexNeighbourCoords(candidate);
                    foreach (Pos2D neighborCoord in neighborCoords)
                    {
                        HexCell neighborCell = mapModel.HexGrid.GetCellByCoord(neighborCoord);
                        for (int i = 0; i < selectedCount; i++)
                        {
                            if (result[i] == neighborCell)
                            {
                                noCenterNearby = false;
                                break;
                            }
                        }
                    }
                }

                if (distanceValid && noCenterNearby)
                {
                    result[selectedCount] = candidate;
                    selectedCount++;
                    allCells.RemoveAt(randomIndex);
                }
                // 检查这个
            }

            // 防止挑选永远无法成立，死循环
            loopCount++;
            if (loopCount > 2000) break;
        }
        if (loopCount > 2000)
        {
            Debug.LogWarning("在限定次数内无法找到足够的合法HexCell，请改参数！传回空，将采用简单随机生成，有风险");
            return new HexCell[0];   
        }
        else
        {
            Debug.Log("InitBiomeStep：找到了合法的起始点");
            return result;
        }
    }
}
