using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public enum EdgeType
{
    DiffBiomeBoundary,        // 不同群系边界
    SameBiomeBoundary,        // 同群系边界
    ExternalBoundary          // 外部边界（空气）
}

public class HexCellEdge : ICanGetUtility
{
    // 全局共享的 障碍物组 和填充配置
    private static EdgeFillConfig fillConfig;
    private static GameObject obstacleGroup;

    public HexCellEdge edgeConnected;                       // 和自己相连的边（共用边）
    public HexDirection direction;                          // 该边在所属HexCell的方向
    public bool IsConnected => edgeConnected != null;       // 是否连接到其他HexCell
    public bool ShouldFill => !(isRoad || isFilled);        // 是否需要填充边界，如果已经被填充或者为道路，则不应该Fill
    
    private bool isFilled;                                  // 是否填充了边界（例如岩石）
    private bool isRoad;                                    // 边界上是否有道路

    // 边界填充相关
    public HexCell ownerCell;                               // 拥有此边界的HexCell
    public HexCell neighbourCell;                           // 相邻的HexCell（如果存在）

    public HexCellEdge(HexDirection direction, HexCell owner)
    {
        this.direction = direction;
        this.ownerCell = owner;
        this.isFilled = false;
        this.isRoad = false;
    }

    #region 外部可用方法

    #region Getters

    /// <summary>
    /// 获取是否为道路
    /// </summary>
    public bool GetIsRoad()
    {
        return isRoad;
    }

    public bool GetIsFilled()
    {
        return isFilled;
    }

    #endregion


    /// <summary>
    /// 全局共享一份即可，设置静态填充配置和障碍物组
    /// </summary>
    public static void SetStaticFillConfig(EdgeFillConfig config, GameObject group)
    {
        fillConfig = config;
        obstacleGroup = group;
    }

    /// <summary>
    /// 设置道路bool状态
    /// </summary>
    public void SetIsRoad(bool isRoad)
    {
        this.isRoad = isRoad;
    }

    /// <summary>
    /// 设置边缘障碍物的填充状态
    /// </summary>
    public void SetIsFilled(bool isFilled)
    {
        this.isFilled = isFilled;
    }

    /// <summary>
    /// 标记两条边共用，并且通知对方
    /// </summary>
    /// <param name="connectedEdge"></param>
    /// <param name="neighbourCell"></param>
    public void SetEdgeConnected(HexCellEdge connectedEdge, HexCell neighbourCell)
    {
        this.edgeConnected = connectedEdge;
        this.neighbourCell = neighbourCell;
        if (connectedEdge != null)
        {
            connectedEdge.edgeConnected = this;
            connectedEdge.neighbourCell = ownerCell;
        }
    }


    #endregion

    #region 外部可用方法

    // 获取Edge的起点和终点
    public (Vector3 startPoint, Vector3 endPoint) GetEdgePoints()
    {
        int startIndex = (int)direction * 2 - 1;  
        int endIndex = (int)direction * 2 + 1;

        // 此处要环形处理，startIndex最开始在索引为11的位置
        if (startIndex < 0) startIndex = 11;

        Vector3 startPoint = ownerCell.GetEdgePointByIndex(startIndex);
        Vector3 endPoint = ownerCell.GetEdgePointByIndex(endIndex);

        return (startPoint, endPoint);
    }



    /// <summary>
    /// 获取边界类型(空气，相同群系，不同群系)
    /// </summary>
    public EdgeType GetEdgeType()
    {
        if (!IsConnected)
        {
            return EdgeType.ExternalBoundary;                // 外部边界（空气）
        }

        if (ownerCell.HexRealm == neighbourCell.HexRealm)
        {
            return EdgeType.SameBiomeBoundary;
        }

        return EdgeType.DiffBiomeBoundary;
    }


    /// <summary>
    /// 获取填充材质
    /// </summary>
    public Material GetFillMaterial()
    {
        EdgeType edgeType = GetEdgeType();

        switch (edgeType)
        {
            case EdgeType.DiffBiomeBoundary:
                // 从俩群系随机挑个材质
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    return ownerCell.HexRealm.GetRealmBiome().CommonMaterial;
                }
                else
                {
                    if (neighbourCell != null && neighbourCell.HexRealm != null)
                    {
                        Biome biome = neighbourCell.HexRealm.GetRealmBiome();
                        if (biome != null && biome.CommonMaterial != null)
                        {
                            return biome.CommonMaterial;
                        }
                    }
                    return ownerCell.HexRealm.GetRealmBiome().CommonMaterial;
                }

            case EdgeType.SameBiomeBoundary:
            case EdgeType.ExternalBoundary:
            default:
                return ownerCell.HexRealm.GetRealmBiome().CommonMaterial;
        }
    }

    #endregion

    #region Architecture
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
