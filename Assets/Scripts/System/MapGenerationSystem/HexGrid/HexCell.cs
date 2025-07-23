using QFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 六边形单元格，自己管理自己的数据和行为，标准为横向摆放（尖端水平朝向）
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexCell : MonoBehaviour, IController
{
    // MVC
    MapGenerationSystem mapGenerationSystem;
    MapModel mapModel;

    // 六边形Cell的唯一标识符
    [Header("寻路属性")]
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

    [Header("连接状态")]
    // 保存六个边界的信息
    public HexCellEdge[] edges;
    // 六条边是否连接到道路
    //public bool[] edgeRoadConnected = new bool[6];
    public HexCell pathParent;
    public HexCell connectedCell;

    // 缓存数据
    public float innerRadius;
    public float outerRadius;
    public Pos2D coord;                     // 六边形坐标

    public bool isOccupied = false;
    public HexRealm HexRealm { get; private set; } = null; // 当前Cell所属的领域

    // Unity 组件引用
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;      // 初始材质
    private Material instanceMaterial;      // 实时材质

    #region 生命周期
    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        // 初始化边界对象
        InitializeEdges();
    }

    private void OnEnable()
    {
        // 缓存数据，放在Start前解决时序问题
        innerRadius = mapModel.HexInnerRadius;
        outerRadius = mapModel.HexOuterRadius;
    }

    private void Start()
    {
      
    }

    private void OnDestroy()
    {
        // 销毁动态创建的Material
        if (instanceMaterial != null)
        {
            DestroyImmediate(instanceMaterial);
        }
    }

    #endregion

    #region 外部修改方法

    #region 初始化方法

    /// <summary>
    /// 在XZ平面初始化一层六边形:世界位置、坐标
    /// </summary>
    public void InitializeXZ(Vector3 position, Pos2D coord)
    {
        transform.localPosition = position;
        this.coord = coord;
    }

    /// <summary>
    /// 设置Cell共享Mesh
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
            Debug.LogWarning("传入的Mesh为空！");
        }
    }

    /// <summary>
    /// 设置HexCell所属的领域
    /// </summary>
    /// <param name="hexRealm"></param>
    public void SetHexRealm(HexRealm hexRealm)
    {
        HexRealm = hexRealm;
    }

    /// <summary>
    /// 初始化六个边界对象
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

    #region 颜色测试，后续填充为地图

    /// <summary>
    /// 设置颜色
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
    /// 重置颜色
    /// </summary>
    public void ResetColor()
    {
        if (instanceMaterial != null)
        {
            instanceMaterial.color = originalMaterial.color;
        }
    }

    #endregion

    #region 六边形相关方法

    /// <summary>
    /// 根据索引获取六边形上的点位置（0-11，每30度一个点。12点方向的索引为0）
    /// </summary>
    public Vector3 GetEdgePointByIndex(int index)
    {
        if (index < 0)
        {
            Debug.LogError("请输入合法索引, index >= 0");
            return Vector3.zero;
        }

        index %= 12;
        float angleDegrees = index * 30f;

        float radius = (index % 2 != 0) ? outerRadius : innerRadius; // 奇数为角，偶数为边
        float x = radius * Mathf.Sin(Mathf.Deg2Rad * angleDegrees);
        float z = radius * Mathf.Cos(Mathf.Deg2Rad * angleDegrees);
        //Debug.Log($"GetEdgePointByIndex: index={index}, angle={angleDegrees}, x={x}, z={z}");
        return transform.position + new Vector3(x, 0, z);
    }

    /// <summary>
    /// 返回每个顶点（index奇数点）和中心连线的线段中点
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetCornerPositionSemi()
    {
        Vector3[] positions = new Vector3[6];
        Vector3 center = transform.position;

        for (int i = 0; i < 6; i++)
        {
            // 奇数索引：1,3,5,7,9,11
            int cornerIndex = i * 2 + 1;
            Vector3 cornerPosition = GetEdgePointByIndex(cornerIndex);
            //Debug.Log($"{center} : {cornerPosition}");
            positions[i] = Vector3.Lerp(center, cornerPosition, 0.62f);
        }

        return positions;
    }

    /// <summary>
    /// 设置占用状态，占用状态代表xz
    /// </summary>
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
    /// <summary>
    /// 根据两个HexCell的坐标自动设置边界连接
    /// </summary>
    /// <param name="targetCell">要连接的邻居Cell</param>
    public void SetEdgeConnection(HexCell targetCell)
    {
        // 获取从当前Cell到邻居Cell的方向
        HexDirection? direction = GetDirectionToNeighbourCell(targetCell);

        if (direction.HasValue)
        {
            int edgeIndex = (int)direction.Value;
            HexDirection oppositeDirection = HexMetrics.GetOppositeDirection(direction.Value);
            int oppositeEdgeIndex = (int)oppositeDirection;
            HexCellEdge thisEdge = edges[edgeIndex];
            HexCellEdge targetEdge = targetCell.edges[oppositeEdgeIndex];

            // 相互绑定边界联通状态
            if (edges != null && edgeIndex < edges.Length)
            {
                thisEdge.edgeConnected = targetEdge; 
                targetEdge.edgeConnected = thisEdge;
            }
        }
        else
        {
            Debug.LogWarning($"Cell ({coord.x},{coord.y}) 和 Cell ({targetCell.coord.x},{targetCell.coord.y}) 不相邻，无法标记Edge公用");
        }
    }

    /// <summary>
    /// 把对应方向的Edge设置为道路
    /// </summary>
    public void SetDirectionEdgeIsRoad(HexDirection direction)
    {
        edges[(int)direction].SetIsRoad(true);
    }

    /// <summary>
    /// 获取从当前Cell到相邻Cell的方向
    /// </summary>
    /// <param name="neighborCell">邻居Cell</param>
    /// <returns>方向，如果不相邻返回null</returns>
    public HexDirection? GetDirectionToNeighbourCell(HexCell neighborCell)
    {
        if (neighborCell == null) return null;

        Pos2D offset = new Pos2D(neighborCell.coord.x - coord.x, neighborCell.coord.y - coord.y);
        return HexMetrics.GetDirectionFromOffset(coord, offset);
    }

    /// <summary>
    /// 检查两个Cell是否相邻
    /// </summary>
    /// <param name="otherCell">另一个Cell</param>
    /// <returns>是否相邻</returns>
    public bool GetIsNeighbourWith(HexCell otherCell)
    {
        return GetDirectionToNeighbourCell(otherCell) != null;
    }

    /// <summary>
    /// 检查并绑定与所有已占用邻居HexCell的边界连接
    /// 当HexCell被添加到Realm时调用此方法
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
                    // 同步更新绑定邻居的信息
                    HexDirection oppositeDirection = HexMetrics.GetOppositeDirection(direction.Value);
                    int edgeIndex = (int)direction.Value;
                    int oppositeEdgeIndex = (int)oppositeDirection;
                    edges[edgeIndex].SetEdgeConnected(neighbour.edges[oppositeEdgeIndex], neighbour);

                    //Debug.Log($"绑定了 Cell({coord.x},{coord.y}) Cell({neighbour.coord.x},{neighbour.coord.y}) 的边界");
                }
            }
        }
    }

    /// <summary>
    /// 获取指定方向上的邻居HexCell（如果存在）
    /// </summary>
    /// <param name="direction">查找方向</param>
    /// <returns>邻居HexCell，如果不存在返回null</returns>
    public HexCell GetNeighborInDirection(HexDirection direction)
    {
        // 计算邻居坐标
        Pos2D neighborCoord = HexMetrics.GetHexCellCoordByDirection(coord, direction);

        // 从HexGrid中查找邻居
        return mapModel.HexGrid.GetCellByCoord(neighborCoord);
    }

    /// <summary>
    /// 检查对应方向的边界是否应该绑定（邻居存在且已被纳入领域）
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