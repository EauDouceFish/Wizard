using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �򻯵�NavMesh�決���� - ֻ�決Ground��
/// �������Ժ��֣�Navmeshȱ�ݹ��࣬���ʺ���Ϊ�������
/// </summary>
//public class NavmeshBakeStep : IMapGenerationStep
//{
//    float xStart;
//    float zStart;
//    float xRange;
//    float zRange;
//    float yRange = 4.5f;

//    public void Execute(MapModel mapModel)
//    {
//        xStart = -4.0f * mapModel.HexOuterRadius;
//        zStart = -4.0f * mapModel.HexOuterRadius;
//        xRange = 1.5f * mapModel.MapWidth - xStart;
//        zRange = 1.5f * mapModel.MapHeight - zStart;

//        BakeNavMesh();
//    }

//    /// <summary>
//    ///  �ڵ�һ���޷�Χ�ں決�決NavMesh
//    /// </summary>
//    private void BakeNavMesh()
//    {
//        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);


//        // ����決�߽� - ����㿪ʼ������һ���ޣ���X��Z���򣩺決
//        Bounds bakeBounds = new Bounds(
//            new Vector3(xStart + xRange * 0.5f, yRange * 0.5f + 0.02f, zStart + zRange * 0.5f),
//            new Vector3(xRange, yRange, zRange)
//        );

//        // �ռ��決Դ
//        List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
//        NavMeshBuilder.CollectSources(bakeBounds, ~0, NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), buildSources);

//        // ����ֻ����Ground��Ķ���
//        List<NavMeshBuildSource> filteredSources = new List<NavMeshBuildSource>();
//        foreach (var source in buildSources)
//        {
//            if (source.component != null)
//            {
//                GameObject go = source.component != null ? source.component.gameObject : null;
//                if (go != null && IsGroundLayerByTag(go))
//                {
//                    filteredSources.Add(source);
//                }
//            }
//        }

//        // ִ��NavMesh�決
//        NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(
//            buildSettings,
//            filteredSources,
//            bakeBounds,
//            Vector3.zero,
//            Quaternion.identity);

//        if (navMeshData != null)
//        {
//            NavMesh.AddNavMeshData(navMeshData);
//            Debug.Log("NavMesh�決���");
//            Debug.Log($"�決Դ����: {filteredSources.Count}");
//        }
//    }


//    private bool IsGroundLayerByTag(GameObject gameObject)
//    {
//        if (gameObject.CompareTag("Ground"))
//        {
//            return true;
//        }

//        return false;
//    }
//}