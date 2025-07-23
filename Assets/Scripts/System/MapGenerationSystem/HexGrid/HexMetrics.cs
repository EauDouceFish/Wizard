using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������ݶ���ָ�긨��������Hexͨ���Ա߳�Ϊ������λ
/// </summary>
public static class HexMetrics
{    
    /// <summary>
    /// ������뾶���߳��������������ζ���
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
    /// ����һ������������λ�ã������������������������λ�õ�ƫ������ע�⣬���Ƿ�����λ�ã�
    /// </summary>
    public static Pos2D[] GetDirectionOffsets(Pos2D coord)
    {
        // ����x������ż��ѡ��������
        if (coord.x % 2 == 0) // ż����
        {
            return new Pos2D[]
            {
                new Pos2D(0, 1),   // ��
                new Pos2D(1, 1),   // ����
                new Pos2D(1, 0),   // ����
                new Pos2D(0, -1),  // ��
                new Pos2D(-1, 0),  // ����
                new Pos2D(-1, 1)   // ����
            };
        }
        else // ������
        {
            return new Pos2D[]
            {
                new Pos2D(0, 1),   // ��
                new Pos2D(1, 0),   // ���� 
                new Pos2D(1, -1),  // ���� 
                new Pos2D(0, -1),  // ��
                new Pos2D(-1, -1), // ���� 
                new Pos2D(-1, 0)   // ���� 
            };
        }
    }

    /// <summary>
    /// ��ȡһ��HexCell�������ھ�����
    /// </summary>
    public static List<Pos2D> GetHexNeighbourCoords(HexCell hexcell)
    {
        List<Pos2D> neighbourCoords = new List<Pos2D>();
        Pos2D[] directions = GetDirectionOffsets(hexcell.coord);

        // ���������ھ�����
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
    /// ����HexCellλ�ã��Լ���Direction��õ�����ƫ�ƣ����ݶ��߾���[ƫ����]��ȡ����
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
    /// ���뷽�򣬻�ȡ�෴����
    /// </summary>
    public static HexDirection GetOppositeDirection(HexDirection direction)
    {
        return (HexDirection)(((int)direction + 3) % 6);
    }

    /// <summary>
    /// ��ȡָ��������Ŀ�귽���ϵ�HexCell����
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
    North = 0,      // ��
    NorthEast = 1,  // ����
    SouthEast = 2,  // ����
    South = 3,      // ��
    SouthWest = 4,  // ����
    NorthWest = 5   // ����
}