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
    private static Mesh sharedHexMesh;  // ���������������
    Vector3[] corners;                  // �����εĽǵ�

    // ��������
    float outerRadius;
    
    #region ��������

    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();
    }

    private void Start()
    {
        // ��������һ��Ҫ��Start�ڸ�ֵ��
        outerRadius = mapModel.HexOuterRadius;

        corners = HexMetrics.GenerateCorners(outerRadius);
        // �����û�й���� HexMesh���ʹ�����
        if (sharedHexMesh == null)
        {
            sharedHexMesh = new Mesh();
            CreateHexMesh(sharedHexMesh);  // ��������������
        }

        // �������������ø� MeshFilter
        GetComponent<MeshFilter>().mesh = sharedHexMesh;
    }

    #endregion

    #region �ⲿ���÷���

    /// <summary>
    ///  Ϊ���� HexCell ���ù�������
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

    #region ˽���߼�����

    /// <summary>
    /// ����������Mesh��ʵ�ʷ���(18�����)
    /// </summary>
    /// <param name="mesh"></param>
    private void CreateHexMesh(Mesh mesh)
    {
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 center = Vector3.zero;

        // �������������Σ�ȷ�������ε�������
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(center, corners[i], corners[i + 1], vertices, triangles);
        }

        // ��������������������õ�����
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    // ���һ�������εĶ������ݺ�������Mesh
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
