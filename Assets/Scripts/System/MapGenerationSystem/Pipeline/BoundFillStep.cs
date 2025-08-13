using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �߽���䲽�� - Ϊÿ����ռ��HexCell�ı߽�����ϰ����װ����
/// </summary>
public class BoundFillStep : IMapGenerationStep
{
    private MapModel mapModel;
    private HexGrid hexGrid;
    private Storage storage;
    private EdgeFillConfig fillConfig;
    
    private GameObject obstacleGroup;
    private GameObject roadObstacleGroup;

    public void Execute(MapModel mapModel)
    {
        this.mapModel = mapModel;
        hexGrid = mapModel.HexGrid;
        storage = mapModel.GetUtility<Storage>();

        // ��ʼ���߽��������
        fillConfig = storage.GetEdgeFillConfig();

        // �����ϰ�����
        CreateObstacleGroup();
        CreateRoadObstacleGroup();

        // ����HexCellEdge�ľ�̬����
        SetEdgeFillStaticConfig();
        // Ϊÿ����ռ�õ�HexCell���߽��ϰ���
        foreach (HexCell cell in hexGrid.allHexCellCoordDict.Values)
        {
            if (cell.isOccupied)
            {
                FillCellEdgeObstacles(cell);
            }
        }
    }


    /// <summary>
    /// �����ϰ�����
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
    /// ����Road�ϰ���ר���飨Ĭ�����أ�
    /// </summary>
    private void CreateRoadObstacleGroup()
    {
        roadObstacleGroup = GameObject.Find("RoadObstacles");
        if (roadObstacleGroup == null)
        {
            roadObstacleGroup = new GameObject("RoadObstacles");
            roadObstacleGroup.transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// ����HexCellEdge�ľ�̬���ã�ͨ������򹫹�������
    /// </summary>
    private void SetEdgeFillStaticConfig()
    {
        // ����HexCellEdge�е�fillConfig��obstacleGroup�Ǿ�̬�ģ���Ҫ��������
        HexCellEdge.SetStaticFillConfig(fillConfig, obstacleGroup);
    }

    /// <summary>
    /// Ϊ����HexCell���߽��ϰ���
    /// </summary>
    private void FillCellEdgeObstacles(HexCell cell)
    {
        cell.CheckAndBindEdgesWithOccupiedNeighbours();

        for (int i = 0; i < 6; i++)
        {
            HexCellEdge edge = cell.edges[i];
            if (edge != null)
            {
                if (edge.GetIsRoad())
                {
                    // Road�ߣ���ر�
                    FillRoadEdgeObstacles(edge);
                }
                else if(!(edge.GetIsFilled() || edge.GetIsRoad()))
                {
                    // ��ͨ�ߣ���̬
                    FillEdgeObstacles(edge);
                }
            }
        }
    }

    #region ���߽��ϰ���

    /// <summary>
    /// �������ʹ��һ��Ԥ�����Ƶ��������㷨
    /// </summary>
    public void FillEdgeObstacles(HexCellEdge edge)
    {
        if (!edge.ShouldFill) return;
        if (fillConfig == null || storage == null)
        {
            Debug.LogWarning("EdgeFillConfig �� Storage δ��ʼ��");
            return;
        }

        // ��ȡ�߽�˵㡢�߽糤������
        var (startPoint, endPoint) = edge.GetEdgePoints();
        Vector3 edgeVector = endPoint - startPoint;
        float edgeLength = edgeVector.magnitude;

        // ��ȡ����
        Material fillMaterial = edge.GetFillMaterial();

        FillAroundEdgeWithObstacles(startPoint, edgeVector, edgeLength, fillMaterial);

        edge.SetIsFilled(true);

        // ����ǹ��ñ߽磬֪ͨ���ڱ߽�
        if (edge.IsConnected && edge.edgeConnected != null)
        {
            edge.edgeConnected.SetIsFilled(true);
        }
    }

    /// <summary>
    /// ΪRoad������ϰ����ר���飩
    /// </summary>
    private void FillRoadEdgeObstacles(HexCellEdge edge)
    {
        if (edge.GetIsFilled()) return; // �����ظ����

        // Ϊ����Road�ߴ�����������
        string containerName = $"RoadEdge_Cell({edge.ownerCell.coord.x},{edge.ownerCell.coord.y})_Dir{edge.direction}";
        GameObject edgeContainer = new GameObject(containerName);
        edgeContainer.transform.SetParent(roadObstacleGroup.transform);

        // �����edgeContainer
        var (startPoint, endPoint) = edge.GetEdgePoints();
        Vector3 edgeVector = endPoint - startPoint;
        float edgeLength = edgeVector.magnitude;
        Material fillMaterial = edge.GetFillMaterial();

        FillAroundEdgeWithObstacles(startPoint, edgeVector, edgeLength, fillMaterial, edgeContainer);

        // ��Edge��ס�Լ����ϰ�������
        edge.BindBattleObstacleContainer(edgeContainer);
        edgeContainer.SetActive(false);
        edge.SetIsFilled(true);

        // ͬ����乲���
        if (edge.IsConnected && edge.edgeConnected != null)
        {
            edge.edgeConnected.SetIsFilled(true);
            edge.edgeConnected.BindBattleObstacleContainer(edgeContainer);
        }
    }



    /// <summary>
    /// ʹ�������������㷨����ϰ���
    /// </summary>
    private void FillAroundEdgeWithObstacles(Vector3 startPoint, Vector3 edgeVector, float edgeLength, Material material, GameObject parentContainer = null)
    {
        GameObject targetParent = parentContainer ?? obstacleGroup;

        GameObject[] obstacleModels = GetEdgeObstacleModels();
        Vector3 currentPosition = startPoint;
        Vector3 edgeDirection = edgeVector.normalized;
        float remainDistance = edgeLength;
        float cumulativeRotation = 0f;

        int fillCount = 0;
        int maxFillCount = fillConfig.maxFillCount;

        while (remainDistance > 0.1f && fillCount < maxFillCount)
        {
            // 1.���ѡ���ϰ���ģ��
            GameObject selectedObstacle = obstacleModels[Random.Range(0, obstacleModels.Length)];

            // 2.����ģ�ͳߴ����ת����
            Bounds bounds = selectedObstacle.GetModelBoundsAABB();
            float modelWidth = bounds.size.x;
            float modelHeight = bounds.size.z;

            // ȷ���ĸ��ǽϳ��ߣ������ر߽����죬�Լ��Ƿ���Ҫ������ת90��
            // �ж�˭�ǳ��ߣ�����Ҫ����ת90���ó��ߴ�ֱ�ڱ߽�
            bool needRotate90 = modelWidth > modelHeight;
            float modelLength = needRotate90 ? modelWidth : modelHeight;

            // 3.�����������
            float stretchRatio = Random.Range(fillConfig.minStretchRatio, fillConfig.maxStretchRatio);
            float actualLength = modelLength * stretchRatio;

            // 4.�������һ�� - ��������һ���ϰ������ʣ����벻�㣬���������������յ�
            bool isLastSegment = (fillCount == maxFillCount - 1) || (actualLength >= remainDistance * 0.8f);
            if (isLastSegment)
            {
                // �������һ�ι���ѹ����+������С�������
                stretchRatio = remainDistance / modelLength;
                if (stretchRatio < 0.9f)
                {
                    stretchRatio += fillConfig.minStretchCorrection;
                }

                stretchRatio = Mathf.Max(stretchRatio, fillConfig.minStretchRatio);

                actualLength = remainDistance;
            }

            // 5.������ת
            float singleRotation = Random.Range(-fillConfig.singleRotationRange, fillConfig.singleRotationRange);
            float newCumulativeRotation = cumulativeRotation + singleRotation;

            // ����ۼ���ת�Ƿ񳬳���ֵ
            if (Mathf.Abs(newCumulativeRotation) > fillConfig.cumulativeRotationRange)
            {
                // ������ֵʱ��ǿ��������ת
                float correctionRotation = -Mathf.Sign(newCumulativeRotation) * fillConfig.singleRotationRange;
                singleRotation = correctionRotation;
                newCumulativeRotation = cumulativeRotation + correctionRotation;
            }

            // ��������һ�Σ�ǿ����ת��0��ȷ�����ӵ��յ�
            if (isLastSegment)
            {
                newCumulativeRotation = 0f;
            }

            cumulativeRotation = newCumulativeRotation;

            // 6.�����ϰ���λ�ú���ת
            // ������ǵ�һ���ϰ����Ҫ��ǰƫ���Ա����϶
            float positionOffset = (fillCount > 0) ? fillConfig.innerOffset : 0f;

            // �ϰ�������λ�ã���ǰλ�� + ��ǰƫ�� + �ر߽緽��*0.5
            Vector3 obstaclePosition = currentPosition - edgeDirection * positionOffset + edgeDirection * (actualLength * 0.5f);

            // �ϰ�����ת���������� + ģ�͵�����ת + �ۼ���תƫ��
            Quaternion baseRotation = Quaternion.LookRotation(edgeDirection);

            if (needRotate90)
            {
                baseRotation *= Quaternion.Euler(0f, 90f, 0f);
            }

            Quaternion obstacleRotation = baseRotation * Quaternion.Euler(0f, cumulativeRotation, 0f);

            // 7.�����ϰ���ʵ��
            GameObject obstacleInstance = Object.Instantiate(selectedObstacle, obstaclePosition, obstacleRotation);
            obstacleInstance.transform.SetParent(targetParent.transform);

            Vector3 scale = obstacleInstance.transform.localScale;


            // �����Ƿ���ת��90�������������ĸ���
            if (needRotate90)
            {
                // ��ת��90�Ⱥ�ԭ����X�ᣨ��ȣ�������Z�᷽����
                // ��Ϊ������Ҫ�����ر߽�ķ�����������X��
                scale = Vector3.Scale(scale, new Vector3(stretchRatio, 1f, 1f));
            }
            else
            {
                // û����ת��ֱ������Z��
                scale = Vector3.Scale(scale, new Vector3(1f, 1f, stretchRatio));
            }

            obstacleInstance.transform.localScale = scale;

            obstacleInstance.SetMaterial(material);

            // 9.����λ�ú�ʣ�����
            // ע�⣺����Ҳ��Ҫ����ƫ������ʵ��ǰ������Ҫ��ȥƫ��
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

    #region ��������

    private GameObject[] GetEdgeObstacleModels()
    {
        return storage.GetAllEdgeObstacleModels();
    }

    #endregion
}