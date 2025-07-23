using QFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// �����ε�Ԫ���Լ������Լ������ݺ���Ϊ����׼Ϊ����ڷţ����ˮƽ����
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexCell : MonoBehaviour, IController
{
    // MVC
    MapGenerationSystem mapGenerationSystem;
    MapModel mapModel;

    // ������Cell��Ψһ��ʶ��
    [Header("Ѱ·����")]
    public float costFromOrigin = 0f;
    public float costToDestination = 0f;
    private int terrainCost = 0;
    public int TerrainCost
    {
        get => isOccupied ? int.MaxValue : terrainCost;
        set
        {
            terrainCost = value;
        }
    }
    public float TotalCost => costFromOrigin + costToDestination + terrainCost;

    [Header("����״̬")]
    // ���������߽����Ϣ
    public HexCellEdge[] edges;
    // �������Ƿ����ӵ���·
    //public bool[] edgeRoadConnected = new bool[6];
    public HexCell pathParent;
    public HexCell connectedCell;

    // ��������
    public float innerRadius;
    public float outerRadius;
    public Pos2D coord;                     // ����������

    public bool isOccupied = false;
    public HexRealm HexRealm { get; private set; } = null; // ��ǰCell����������

    // Unity �������
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;      // ��ʼ����
    private Material instanceMaterial;      // ʵʱ����

    #region ��������
    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        // ��ʼ���߽����
        InitializeEdges();
    }

    private void OnEnable()
    {
        // �������ݣ�����Startǰ���ʱ������
        innerRadius = mapModel.HexInnerRadius;
        outerRadius = mapModel.HexOuterRadius;
    }

    private void Start()
    {
      
    }

    private void OnDestroy()
    {
        // ���ٶ�̬������Material
        if (instanceMaterial != null)
        {
            DestroyImmediate(instanceMaterial);
        }
    }

    #endregion

    #region �ⲿ�޸ķ���

    #region ��ʼ������

    /// <summary>
    /// ��XZƽ���ʼ��һ��������:����λ�á�����
    /// </summary>
    public void InitializeXZ(Vector3 position, Pos2D coord)
    {
        transform.localPosition = position;
        this.coord = coord;
    }

    /// <summary>
    /// ����Cell����Mesh
    /// </summary>
    /// <param name="sharedMesh"></param>
    public void SetSharedMesh(Mesh sharedMesh)
    {
        if (sharedMesh != null)
        {
            meshFilter.mesh = sharedMesh;
            meshCollider.sharedMesh = sharedMesh;
        }
        else
        {
            Debug.LogWarning("�����MeshΪ�գ�");
        }
    }

    /// <summary>
    /// ����HexCell����������
    /// </summary>
    /// <param name="hexRealm"></param>
    public void SetHexRealm(HexRealm hexRealm)
    {
        HexRealm = hexRealm;
    }

    /// <summary>
    /// ��ʼ�������߽����
    /// </summary>
    private void InitializeEdges()
    {
        edges = new HexCellEdge[6];
        for (int i = 0; i < 6; i++)
        {
            edges[i] = new HexCellEdge((HexDirection)i, this);
        }
    }

    #endregion

    #region ��ɫ���ԣ��������Ϊ��ͼ

    /// <summary>
    /// ������ɫ
    /// </summary>
    public void SetColor(Color color)
    {
        if (instanceMaterial == null)
        {
            instanceMaterial = new Material(originalMaterial);
            meshRenderer.material = instanceMaterial;
        }

        instanceMaterial.color = new Color(color.r, color.g, color.b, instanceMaterial.color.a);
    }

    /// <summary>
    /// ������ɫ
    /// </summary>
    public void ResetColor()
    {
        if (instanceMaterial != null)
        {
            instanceMaterial.color = originalMaterial.color;
        }
    }

    #endregion

    #region ��������ط���

    /// <summary>
    /// ����������ȡ�������ϵĵ�λ�ã�0-11��ÿ30��һ���㡣12�㷽�������Ϊ0��
    /// </summary>
    public Vector3 GetEdgePointByIndex(int index)
    {
        if (index < 0)
        {
            Debug.LogError("������Ϸ�����, index >= 0");
            return Vector3.zero;
        }

        index %= 12;
        float angleDegrees = index * 30f;

        float radius = (index % 2 != 0) ? outerRadius : innerRadius; // ����Ϊ�ǣ�ż��Ϊ��
        float x = radius * Mathf.Sin(Mathf.Deg2Rad * angleDegrees);
        float z = radius * Mathf.Cos(Mathf.Deg2Rad * angleDegrees);
        //Debug.Log($"GetEdgePointByIndex: index={index}, angle={angleDegrees}, x={x}, z={z}");
        return transform.position + new Vector3(x, 0, z);
    }

    /// <summary>
    /// ����ÿ�����㣨index�����㣩���������ߵ��߶��е�
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetCornerPositionSemi()
    {
        Vector3[] positions = new Vector3[6];
        Vector3 center = transform.position;

        for (int i = 0; i < 6; i++)
        {
            // ����������1,3,5,7,9,11
            int cornerIndex = i * 2 + 1;
            Vector3 cornerPosition = GetEdgePointByIndex(cornerIndex);
            //Debug.Log($"{center} : {cornerPosition}");
            positions[i] = Vector3.Lerp(center, cornerPosition, 0.62f);
        }

        return positions;
    }

    /// <summary>
    /// ����ռ��״̬��ռ��״̬����xz
    /// </summary>
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
    /// <summary>
    /// ��������HexCell�������Զ����ñ߽�����
    /// </summary>
    /// <param name="targetCell">Ҫ���ӵ��ھ�Cell</param>
    public void SetEdgeConnection(HexCell targetCell)
    {
        // ��ȡ�ӵ�ǰCell���ھ�Cell�ķ���
        HexDirection? direction = GetDirectionToNeighbourCell(targetCell);

        if (direction.HasValue)
        {
            int edgeIndex = (int)direction.Value;
            HexDirection oppositeDirection = HexMetrics.GetOppositeDirection(direction.Value);
            int oppositeEdgeIndex = (int)oppositeDirection;
            HexCellEdge thisEdge = edges[edgeIndex];
            HexCellEdge targetEdge = targetCell.edges[oppositeEdgeIndex];

            // �໥�󶨱߽���ͨ״̬
            if (edges != null && edgeIndex < edges.Length)
            {
                thisEdge.edgeConnected = targetEdge; 
                targetEdge.edgeConnected = thisEdge;
            }
        }
        else
        {
            Debug.LogWarning($"Cell ({coord.x},{coord.y}) �� Cell ({targetCell.coord.x},{targetCell.coord.y}) �����ڣ��޷����Edge����");
        }
    }

    /// <summary>
    /// �Ѷ�Ӧ�����Edge����Ϊ��·
    /// </summary>
    public void SetDirectionEdgeIsRoad(HexDirection direction)
    {
        edges[(int)direction].SetIsRoad(true);
    }

    /// <summary>
    /// ��ȡ�ӵ�ǰCell������Cell�ķ���
    /// </summary>
    /// <param name="neighborCell">�ھ�Cell</param>
    /// <returns>������������ڷ���null</returns>
    public HexDirection? GetDirectionToNeighbourCell(HexCell neighborCell)
    {
        if (neighborCell == null) return null;

        Pos2D offset = new Pos2D(neighborCell.coord.x - coord.x, neighborCell.coord.y - coord.y);
        return HexMetrics.GetDirectionFromOffset(coord, offset);
    }

    /// <summary>
    /// �������Cell�Ƿ�����
    /// </summary>
    /// <param name="otherCell">��һ��Cell</param>
    /// <returns>�Ƿ�����</returns>
    public bool GetIsNeighbourWith(HexCell otherCell)
    {
        return GetDirectionToNeighbourCell(otherCell) != null;
    }

    /// <summary>
    /// ��鲢����������ռ���ھ�HexCell�ı߽�����
    /// ��HexCell����ӵ�Realmʱ���ô˷���
    /// </summary>
    public void CheckAndBindEdgesWithOccupiedNeighbours()
    {
        List<HexCell> neighbours = mapModel.HexGrid.GetHexCellPathFinder().GetNeighborHexCells(this);

        foreach (HexCell neighbour in neighbours)
        {
            if (neighbour.isOccupied)
            {
                SetEdgeConnection(neighbour);
                HexDirection? direction = GetDirectionToNeighbourCell(neighbour);
                if (direction.HasValue)
                {
                    // ͬ�����°��ھӵ���Ϣ
                    HexDirection oppositeDirection = HexMetrics.GetOppositeDirection(direction.Value);
                    int edgeIndex = (int)direction.Value;
                    int oppositeEdgeIndex = (int)oppositeDirection;
                    edges[edgeIndex].SetEdgeConnected(neighbour.edges[oppositeEdgeIndex], neighbour);

                    //Debug.Log($"���� Cell({coord.x},{coord.y}) Cell({neighbour.coord.x},{neighbour.coord.y}) �ı߽�");
                }
            }
        }
    }

    /// <summary>
    /// ��ȡָ�������ϵ��ھ�HexCell��������ڣ�
    /// </summary>
    /// <param name="direction">���ҷ���</param>
    /// <returns>�ھ�HexCell����������ڷ���null</returns>
    public HexCell GetNeighborInDirection(HexDirection direction)
    {
        // �����ھ�����
        Pos2D neighborCoord = HexMetrics.GetHexCellCoordByDirection(coord, direction);

        // ��HexGrid�в����ھ�
        return mapModel.HexGrid.GetCellByCoord(neighborCoord);
    }

    /// <summary>
    /// ����Ӧ����ı߽��Ƿ�Ӧ�ð󶨣��ھӴ������ѱ���������
    /// </summary>
    public bool ShouldBindEdgeInDirection(HexDirection direction)
    {
        HexCell neighbor = GetNeighborInDirection(direction);
        return neighbor != null && neighbor.isOccupied;
    }

    #endregion

    #endregion

    #region Interface
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}