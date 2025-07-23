using UnityEngine;
using System.Collections.Generic;
using QFramework;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour, IController
{
    // MVC
    MapModel mapModel;          
    MapGenerationSystem mapGenerationSystem;
    private static Mesh sharedHexMesh;  // 共享的六边形网格
    Vector3[] corners;                  // 六边形的角点

    // 缓存数据
    float outerRadius;
    
    #region 生命周期

    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();
    }

    private void Start()
    {
        // 缓存数据一定要在Start内赋值！
        outerRadius = mapModel.HexOuterRadius;

        corners = HexMetrics.GenerateCorners(outerRadius);
        // 如果还没有共享的 HexMesh，就创建它
        if (sharedHexMesh == null)
        {
            sharedHexMesh = new Mesh();
            CreateHexMesh(sharedHexMesh);  // 创建六边形网格
        }

        // 将共享网格设置给 MeshFilter
        GetComponent<MeshFilter>().mesh = sharedHexMesh;
    }

    #endregion

    #region 外部可用方法

    /// <summary>
    ///  为所有 HexCell 设置共享网格
    /// </summary>
    /// <param name="cells"></param>
    public void Triangulate(HexCell[] cells)
    {
        foreach (var cell in cells)
        {
            cell.SetSharedMesh(sharedHexMesh);
        }
    }

    #endregion

    #region 私有逻辑方法

    /// <summary>
    /// 创建六边形Mesh的实际方法(18顶点版)
    /// </summary>
    /// <param name="mesh"></param>
    private void CreateHexMesh(Mesh mesh)
    {
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 center = Vector3.zero;

        // 创建六个三角形，确保六边形的完整性
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(center, corners[i], corners[i + 1], vertices, triangles);
        }

        // 将顶点和三角形数据设置到网格
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    // 添加一个三角形的顶点数据和索引到Mesh
    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, List<Vector3> vertices, List<int> triangles)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    #endregion

    #region Interface
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
