using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 初始化群系起始位置
/// </summary>
public class ConnectMainPathStep : IMapGenerationStep
{
    private MapModel mapModel;
    public void Execute(MapModel mapModel)
    {
        this.mapModel = mapModel;
        List<HexRealm> hexRealms = mapModel.HexGrid.GetHexRealms();
        for (int i = 0; i < hexRealms.Count-1; i++)
        {
            ConnectHexRealmSymmetrically(hexRealms[i], hexRealms[i + 1]);
        }
    }

    /// <summary>
    /// 连接两个领域的初始位置，轮流向Path中心填充HexCell
    /// </summary>
    public Path ConnectHexRealmSymmetrically(HexRealm hexRealm1, HexRealm hexRealm2)
    {
        // 获取两个领域初始的联通路径
        HexCell initCell1 = hexRealm1.GetInitHexCell();
        HexCell initCell2 = hexRealm2.GetInitHexCell();
        Path path = mapModel.HexGrid.GetHexCellPathFinder().FindPath(initCell1, initCell2);
        PathFinderHelper.SetRoadForPath(path);
        //VisualizePath(path, Color.yellow);
        // ModifyCircleSummonAvailablePos(path); 生成Group
        Debug.Log($"生成了 {initCell1.HexRealm.GetRealmBiome().BiomeName} 到 {initCell2.HexRealm.GetRealmBiome().BiomeName} 联通路径，长度为{path.hexCells.Length}");
        int left = 0;
        int right = path.hexCells.Length - 1;
        while (left <= right)
        {
            // 格子相同就随机选择，格子不同就分别赋值给两个领域
            if (left == right)
            {
                int ramdomChoose = Random.Range(0, 2);
                if (ramdomChoose == 0)
                {
                    hexRealm1.AddHexCellIntoRealm(path.hexCells[left]);
                }
                else
                {
                    hexRealm2.AddHexCellIntoRealm(path.hexCells[left]);
                }
            }
            else
            {
                hexRealm1.AddHexCellIntoRealm(path.hexCells[left]);
                hexRealm2.AddHexCellIntoRealm(path.hexCells[right]);
            }

            left++;
            right--;
        }
        return path;
    }
}
