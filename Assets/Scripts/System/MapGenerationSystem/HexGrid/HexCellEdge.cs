using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public enum EdgeType
{
    DiffBiomeBoundary,        // ��ͬȺϵ�߽�
    SameBiomeBoundary,        // ͬȺϵ�߽�
    ExternalBoundary          // �ⲿ�߽磨������
}

public class HexCellEdge : ICanGetUtility
{
    // ȫ�ֹ���� �ϰ����� ���������
    private static EdgeFillConfig fillConfig;
    private static GameObject obstacleGroup;

    public HexCellEdge edgeConnected;                       // ���Լ������ıߣ����ñߣ�
    public HexDirection direction;                          // �ñ�������HexCell�ķ���
    public bool IsConnected => edgeConnected != null;       // �Ƿ����ӵ�����HexCell
    public bool ShouldFill => !(isRoad || isFilled);        // �Ƿ���Ҫ���߽磬����Ѿ���������Ϊ��·����Ӧ��Fill
    
    private bool isFilled;                                  // �Ƿ�����˱߽磨������ʯ��
    private bool isRoad;                                    // �߽����Ƿ��е�·

    // �߽�������
    public HexCell ownerCell;                               // ӵ�д˱߽��HexCell
    public HexCell neighbourCell;                           // ���ڵ�HexCell��������ڣ�

    public HexCellEdge(HexDirection direction, HexCell owner)
    {
        this.direction = direction;
        this.ownerCell = owner;
        this.isFilled = false;
        this.isRoad = false;
    }

    #region �ⲿ���÷���

    #region Getters

    /// <summary>
    /// ��ȡ�Ƿ�Ϊ��·
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
    /// ȫ�ֹ���һ�ݼ��ɣ����þ�̬������ú��ϰ�����
    /// </summary>
    public static void SetStaticFillConfig(EdgeFillConfig config, GameObject group)
    {
        fillConfig = config;
        obstacleGroup = group;
    }

    /// <summary>
    /// ���õ�·bool״̬
    /// </summary>
    public void SetIsRoad(bool isRoad)
    {
        this.isRoad = isRoad;
    }

    /// <summary>
    /// ���ñ�Ե�ϰ�������״̬
    /// </summary>
    public void SetIsFilled(bool isFilled)
    {
        this.isFilled = isFilled;
    }

    /// <summary>
    /// ��������߹��ã�����֪ͨ�Է�
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

    #region �ⲿ���÷���

    // ��ȡEdge�������յ�
    public (Vector3 startPoint, Vector3 endPoint) GetEdgePoints()
    {
        int startIndex = (int)direction * 2 - 1;  
        int endIndex = (int)direction * 2 + 1;

        // �˴�Ҫ���δ���startIndex�ʼ������Ϊ11��λ��
        if (startIndex < 0) startIndex = 11;

        Vector3 startPoint = ownerCell.GetEdgePointByIndex(startIndex);
        Vector3 endPoint = ownerCell.GetEdgePointByIndex(endIndex);

        return (startPoint, endPoint);
    }



    /// <summary>
    /// ��ȡ�߽�����(��������ͬȺϵ����ͬȺϵ)
    /// </summary>
    public EdgeType GetEdgeType()
    {
        if (!IsConnected)
        {
            return EdgeType.ExternalBoundary;                // �ⲿ�߽磨������
        }

        if (ownerCell.HexRealm == neighbourCell.HexRealm)
        {
            return EdgeType.SameBiomeBoundary;
        }

        return EdgeType.DiffBiomeBoundary;
    }


    /// <summary>
    /// ��ȡ������
    /// </summary>
    public Material GetFillMaterial()
    {
        EdgeType edgeType = GetEdgeType();

        switch (edgeType)
        {
            case EdgeType.DiffBiomeBoundary:
                // ����Ⱥϵ�����������
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
