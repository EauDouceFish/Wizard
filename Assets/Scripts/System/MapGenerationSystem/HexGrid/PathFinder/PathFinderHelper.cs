using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对六边形网格路径进行辅助操作的类
/// </summary>
public static class PathFinderHelper
{
    /// <summary>
    /// 为两个相邻的HexCell之间设置道路边
    /// </summary>
    public static void SetRoadBetweenCells(HexCell fromCell, HexCell toCell)
    {
        if (fromCell == null || toCell == null)
        {
            Debug.Log("不能传入空HexCell");
            return;
        }

        HexDirection? direction = fromCell.GetDirectionToNeighbourCell(toCell);
        if (direction == null)
        {
            Debug.Log("需要传入相邻的HexCell");
        }
        HexDirection oppositeDirection = HexMetrics.GetOppositeDirection(direction.Value);

        fromCell.SetDirectionEdgeIsRoad(direction.Value);
        toCell.SetDirectionEdgeIsRoad(oppositeDirection);
    }

    /// <summary>
    /// 把整条Path设置为道路
    /// </summary>
    public static void SetRoadForPath(Path path)
    {
        if (path?.hexCells == null || path.hexCells.Length < 2)
        {
            return;
        }

        for (int i = 0; i < path.hexCells.Length - 1; i++)
        {
            SetRoadBetweenCells(path.hexCells[i], path.hexCells[i + 1]);   
        }
    }
}
