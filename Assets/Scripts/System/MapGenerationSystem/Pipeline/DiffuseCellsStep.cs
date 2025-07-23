using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class DiffuseCellsStep : IMapGenerationStep
{
    MapModel mapModel;
    HexGrid hexGrid;
    public void Execute(MapModel mapModel)
    {
        this.mapModel = mapModel;
        hexGrid = mapModel.HexGrid;
        DiffuseHexRealmCells();
    }

    // 将网格向外扩散，直到达到地图标准
    private void DiffuseHexRealmCells()
    {
        int mapNeedingCellNum = 50;  // 地图大小标准下要求的格子数量，未来由mapModel初始化
        int currentHexCellSum = 0;

        var realms = mapModel.HexGrid.GetHexRealms();

        // 统计平均值
        foreach (HexRealm realm in realms)
        {
            currentHexCellSum += realm.GetHexRealmSize();
        }

        int averageHexCellNum;
        int whileCount = 0; // 避免死循环修正，避免在没有规定限制起始HexCell位置下，某些HexCell被360度包围，导致无法扩散
        while (currentHexCellSum < mapNeedingCellNum)
        {
            if (whileCount > 1000)
            {
                Debug.LogWarning("错误的地图，某些Cell由于随机无受限，导致无法按照规则生成");
                break;
            }
            // 计算HexRealm拥有HexCell数量的平均值，如果当前领域的数量在平均值之下，就将其尝试扩散
            foreach (HexRealm realm in realms)
            {
                averageHexCellNum = currentHexCellSum / realms.Count;
                if (realm.GetHexRealmSize() <= averageHexCellNum)
                {
                    int expandNum = ExpandRealm(realm);
                    currentHexCellSum += expandNum;
                }
            }
            whileCount++;
        }
        //Debug.Log(currentHexCellSum);
    }

    // 膨胀拓展某个领域n个地块
    private int ExpandRealm(HexRealm hexRealm, int num = 1)
    {
        List<HexCell> allHexCells = hexRealm.GetHexCellsInRealm();
        int availableCellsFound = 0;

        for (int i = 0; i < num; i++)
        {
            HashSet<HexCell> candidateSet = new HashSet<HexCell>();
            Dictionary<HexCell, HexCell> expansionMapping = new Dictionary<HexCell, HexCell>();

            foreach (HexCell cell in allHexCells)
            {
                List<HexCell> neighbors = hexGrid.GetHexCellPathFinder().GetNeighborHexCells(cell);
                foreach (HexCell candidate in neighbors)
                {
                    if (!candidate.isOccupied)
                    {
                        candidateSet.Add(candidate);

                        // 记录拓展来源
                        expansionMapping[candidate] = cell;
                    }
                }
            }

            if (candidateSet.Count == 0) return availableCellsFound;

            // 随机选择一个地块进行拓展
            List<HexCell> availableCellsList = new List<HexCell>(candidateSet);
            int randomHexCellIdx = UnityEngine.Random.Range(0, availableCellsList.Count);
            HexCell newCell = availableCellsList[randomHexCellIdx];

            // 获取这个新拓展的地块是由哪个旧地块扩展的、设置为路
            HexCell sourceCell = expansionMapping[newCell];

            PathFinderHelper.SetRoadBetweenCells(sourceCell, newCell);

            // 加入领域
            hexRealm.AddHexCellIntoRealm(newCell);
            availableCellsFound++;
        }

        return availableCellsFound;
    }

}


