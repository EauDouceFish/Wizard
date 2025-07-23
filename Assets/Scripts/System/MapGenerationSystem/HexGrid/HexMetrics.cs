using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形数据度量指标辅助方法，Hex通常以边长为基础单位
/// </summary>
public static class HexMetrics
{    
    /// <summary>
    /// 传入外半径（边长）来构建六边形度量
    /// </summary>
    public static Vector3[] GenerateCorners(float outerRadius)
    {
        var corners = new Vector3[7];
        float halfOuterRadius = outerRadius / 2f;
        var innerRadius = outerRadius * 0.866025404f;
        corners[0] = new Vector3(-halfOuterRadius, 0f, innerRadius);
        corners[1] = new Vector3(halfOuterRadius, 0f, innerRadius);
        corners[2] = new Vector3(outerRadius, 0f, 0f);
        corners[3] = new Vector3(halfOuterRadius, 0f, -innerRadius);
        corners[4] = new Vector3(-halfOuterRadius, 0f, -innerRadius);
        corners[5] = new Vector3(-outerRadius, 0f, 0f);
        corners[6] = corners[0];
        return corners;
    }

    /// <summary>
    /// 输入一个六边形坐标位置，返回其相邻六个方向的坐标位置的偏移量（注意，不是返回新位置）
    /// </summary>
    public static Pos2D[] GetDirectionOffsets(Pos2D coord)
    {
        // 根据x坐标奇偶性选择方向数组
        if (coord.x % 2 == 0) // 偶数列
        {
            return new Pos2D[]
            {
                new Pos2D(0, 1),   // 上
                new Pos2D(1, 1),   // 右上
                new Pos2D(1, 0),   // 右下
                new Pos2D(0, -1),  // 下
                new Pos2D(-1, 0),  // 左下
                new Pos2D(-1, 1)   // 左上
            };
        }
        else // 奇数列
        {
            return new Pos2D[]
            {
                new Pos2D(0, 1),   // 上
                new Pos2D(1, 0),   // 右上 
                new Pos2D(1, -1),  // 右下 
                new Pos2D(0, -1),  // 下
                new Pos2D(-1, -1), // 左下 
                new Pos2D(-1, 0)   // 左上 
            };
        }
    }

    /// <summary>
    /// 获取一个HexCell的所有邻居坐标
    /// </summary>
    public static List<Pos2D> GetHexNeighbourCoords(HexCell hexcell)
    {
        List<Pos2D> neighbourCoords = new List<Pos2D>();
        Pos2D[] directions = GetDirectionOffsets(hexcell.coord);

        // 计算所有邻居坐标
        foreach (var direction in directions)
        {
            Pos2D neighbourCoord = new Pos2D(
                hexcell.coord.x + direction.x,
                hexcell.coord.y + direction.y
            );
            neighbourCoords.Add(neighbourCoord);
        }

        return neighbourCoords;
    }

    /// <summary>
    /// 传入HexCell位置，以及由Direction获得的坐标偏移，根据二者距离[偏移量]获取方向
    /// </summary>
    public static HexDirection? GetDirectionFromOffset(Pos2D fromCoord, Pos2D offset)
    {
        Pos2D[] directions = GetDirectionOffsets(fromCoord);

        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i].x == offset.x && directions[i].y == offset.y)
            {
                return (HexDirection)i;
            }
        }

        return null;
    }

    /// <summary>
    /// 输入方向，获取相反方向
    /// </summary>
    public static HexDirection GetOppositeDirection(HexDirection direction)
    {
        return (HexDirection)(((int)direction + 3) % 6);
    }

    /// <summary>
    /// 获取指定坐标在目标方向上的HexCell坐标
    /// </summary>
    public static Pos2D GetHexCellCoordByDirection(Pos2D curCoord, HexDirection direction)
    {
        Pos2D[] directions = GetDirectionOffsets(curCoord);
        Pos2D offset = directions[(int)direction];

        return new Pos2D(curCoord.x + offset.x, curCoord.y + offset.y);
    }
}

public enum HexDirection
{
    North = 0,      // 上
    NorthEast = 1,  // 右上
    SouthEast = 2,  // 右下
    South = 3,      // 下
    SouthWest = 4,  // 左下
    NorthWest = 5   // 左上
}