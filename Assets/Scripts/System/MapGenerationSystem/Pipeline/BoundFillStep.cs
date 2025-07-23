using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// 边界填充步骤 - 为每个已占用HexCell的边界填充障碍物和装饰物
/// </summary>
public class BoundFillStep : IMapGenerationStep
{
    private HexGrid hexGrid;
    private Storage storage;
    private EdgeFillConfig fillConfig;
    private GameObject obstacleGroup;

    public void Execute(MapModel mapModel)
    {
        hexGrid = mapModel.HexGrid;
        storage = mapModel.GetUtility<Storage>();

        // 初始化边界填充配置
        fillConfig = storage.GetEdgeFillConfig();

        // 创建障碍物组
        CreateObstacleGroup();

        // 设置HexCellEdge的静态配置
        SetEdgeFillStaticConfig();

        // 为每个已占用的HexCell填充边界障碍物
        foreach (HexCell cell in hexGrid.allHexCellCoordDict.Values)
        {
            if (cell.isOccupied)
            {
                FillCellEdgeObstacles(cell);
            }
        }
    }


    /// <summary>
    /// 创建障碍物组
    /// </summary>
    private void CreateObstacleGroup()
    {
        obstacleGroup = GameObject.Find("EdgeObstacles");
        if (obstacleGroup == null)
        {
            obstacleGroup = new GameObject("EdgeObstacles");
            obstacleGroup.transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// 设置HexCellEdge的静态配置（通过反射或公共方法）
    /// </summary>
    private void SetEdgeFillStaticConfig()
    {
        // 由于HexCellEdge中的fillConfig和obstacleGroup是静态的，需要设置它们
        HexCellEdge.SetStaticFillConfig(fillConfig, obstacleGroup);
    }

    /// <summary>
    /// 为单个HexCell填充边界障碍物
    /// </summary>
    private void FillCellEdgeObstacles(HexCell cell)
    {
        cell.CheckAndBindEdgesWithOccupiedNeighbours();

        for (int i = 0; i < 6; i++)
        {
            HexCellEdge edge = cell.edges[i];
            if (edge != null)
            {
                // 填充边界障碍物
                FillEdgeObstacles(edge);

                // edge.FillDecorations();
            }
        }
    }

    #region 填充边界障碍物

    /// <summary>
    /// 填充自身，使用一套预设限制的随机填充算法
    /// </summary>
    public void FillEdgeObstacles(HexCellEdge edge)
    {
        if (!edge.ShouldFill) return;
        if (fillConfig == null || storage == null)
        {
            Debug.LogWarning("EdgeFillConfig 或 Storage 未初始化");
            return;
        }

        // 获取边界端点、边界长度向量
        var (startPoint, endPoint) = edge.GetEdgePoints();
        Vector3 edgeVector = endPoint - startPoint;
        float edgeLength = edgeVector.magnitude;

        // 获取材质
        Material fillMaterial = edge.GetFillMaterial();

        FillAroundEdgeWithObstacles(startPoint, edgeVector, edgeLength, fillMaterial);

        edge.SetIsFilled(true);

        // 如果是共用边界，通知相邻边界
        if (edge.IsConnected && edge.edgeConnected != null)
        {
            edge.edgeConnected.SetIsFilled(true);
        }
    }


    /// <summary>
    /// 使用受限随机填充算法填充障碍物
    /// </summary>
    private void FillAroundEdgeWithObstacles(Vector3 startPoint, Vector3 edgeVector, float edgeLength, Material material)
    {
        GameObject[] obstacleModels = GetEdgeObstacleModels();

        Vector3 currentPosition = startPoint;
        Vector3 edgeDirection = edgeVector.normalized;
        float remainDistance = edgeLength;
        float cumulativeRotation = 0f;

        int fillCount = 0;
        int maxFillCount = fillConfig.maxFillCount;

        while (remainDistance > 0.1f && fillCount < maxFillCount)
        {
            // 1.随机选择障碍物模型
            GameObject selectedObstacle = obstacleModels[Random.Range(0, obstacleModels.Length)];

            // 2.计算模型尺寸和旋转调整
            Bounds bounds = selectedObstacle.GetModelBoundsAABB();
            float modelWidth = bounds.size.x;
            float modelHeight = bounds.size.z;

            // 确定哪个是较长边，用于沿边界拉伸，以及是否需要额外旋转90度
            // 判断谁是长边，有需要就旋转90度让长边垂直于边界
            bool needRotate90 = modelWidth > modelHeight;
            float modelLength = needRotate90 ? modelWidth : modelHeight;

            // 3.计算拉伸比例
            float stretchRatio = Random.Range(fillConfig.minStretchRatio, fillConfig.maxStretchRatio);
            float actualLength = modelLength * stretchRatio;

            // 4.处理最后一段 - 如果是最后一个障碍物或者剩余距离不足，调整到正好连接终点
            bool isLastSegment = (fillCount == maxFillCount - 1) || (actualLength >= remainDistance * 0.8f);
            if (isLastSegment)
            {
                // 避免最后一段过度压缩，+保持最小拉伸比例
                stretchRatio = remainDistance / modelLength;
                if (stretchRatio < 0.9f)
                {
                    stretchRatio += fillConfig.minStretchCorrection;
                }

                stretchRatio = Mathf.Max(stretchRatio, fillConfig.minStretchRatio);

                actualLength = remainDistance;
            }

            // 5.计算旋转
            float singleRotation = Random.Range(-fillConfig.singleRotationRange, fillConfig.singleRotationRange);
            float newCumulativeRotation = cumulativeRotation + singleRotation;

            // 检查累计旋转是否超出阈值
            if (Mathf.Abs(newCumulativeRotation) > fillConfig.cumulativeRotationRange)
            {
                // 超出阈值时，强制往回旋转
                float correctionRotation = -Mathf.Sign(newCumulativeRotation) * fillConfig.singleRotationRange;
                singleRotation = correctionRotation;
                newCumulativeRotation = cumulativeRotation + correctionRotation;
            }

            // 如果是最后一段，强制旋转回0以确保连接到终点
            if (isLastSegment)
            {
                newCumulativeRotation = 0f;
            }

            cumulativeRotation = newCumulativeRotation;

            // 6.计算障碍物位置和旋转
            // 如果不是第一个障碍物，需要向前偏移以避免缝隙
            float positionOffset = (fillCount > 0) ? fillConfig.innerOffset : 0f;

            // 障碍物中心位置：当前位置 + 向前偏移 + 沿边界方向*0.5
            Vector3 obstaclePosition = currentPosition - edgeDirection * positionOffset + edgeDirection * (actualLength * 0.5f);

            // 障碍物旋转：基础朝向 + 模型调整旋转 + 累计旋转偏移
            Quaternion baseRotation = Quaternion.LookRotation(edgeDirection);

            if (needRotate90)
            {
                baseRotation *= Quaternion.Euler(0f, 90f, 0f);
            }

            Quaternion obstacleRotation = baseRotation * Quaternion.Euler(0f, cumulativeRotation, 0f);

            // 7.创建障碍物实例
            GameObject obstacleInstance = Object.Instantiate(selectedObstacle, obstaclePosition, obstacleRotation);
            obstacleInstance.transform.SetParent(obstacleGroup.transform);

            Vector3 scale = obstacleInstance.transform.localScale;

            // 根据是否旋转了90度来决定拉伸哪个轴
            if (needRotate90)
            {
                // 旋转了90度后，原来的X轴（宽度）现在在Z轴方向上
                // 因为我们想要拉伸沿边界的方向，所以拉伸X轴
                scale = Vector3.Scale(scale, new Vector3(stretchRatio, 1f, 1f));
            }
            else
            {
                // 没有旋转，直接拉伸Z轴
                scale = Vector3.Scale(scale, new Vector3(1f, 1f, stretchRatio));
            }

            obstacleInstance.transform.localScale = scale;

            obstacleInstance.SetMaterial(material);

            // 9.更新位置和剩余距离
            // 注意：这里也需要考虑偏移量，实际前进距离要减去偏移
            float actualMoveDistance = actualLength - positionOffset;
            currentPosition += edgeDirection * actualMoveDistance;
            remainDistance -= actualMoveDistance;
            fillCount++;

            if (isLastSegment)
            {
                break;
            }
        }
    }

    #endregion

    #region 辅助方法

    private GameObject[] GetEdgeObstacleModels()
    {
        return storage.GetAllEdgeObstacleModels();
    }

    #endregion
}