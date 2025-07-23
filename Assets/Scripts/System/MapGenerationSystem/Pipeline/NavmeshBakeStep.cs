using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 简化的NavMesh烘焙步骤 - 只烘焙Ground层
/// 尝试了以后发现，Navmesh缺陷过多，不适合作为解决方案
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
//    ///  在第一象限范围内烘焙烘焙NavMesh
//    /// </summary>
//    private void BakeNavMesh()
//    {
//        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);


//        // 定义烘焙边界 - 从起点开始，往第一象限（正X正Z方向）烘焙
//        Bounds bakeBounds = new Bounds(
//            new Vector3(xStart + xRange * 0.5f, yRange * 0.5f + 0.02f, zStart + zRange * 0.5f),
//            new Vector3(xRange, yRange, zRange)
//        );

//        // 收集烘焙源
//        List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
//        NavMeshBuilder.CollectSources(bakeBounds, ~0, NavMeshCollectGeometry.RenderMeshes, 0, new List<NavMeshBuildMarkup>(), buildSources);

//        // 过滤只包含Ground层的对象
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

//        // 执行NavMesh烘焙
//        NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(
//            buildSettings,
//            filteredSources,
//            bakeBounds,
//            Vector3.zero,
//            Quaternion.identity);

//        if (navMeshData != null)
//        {
//            NavMesh.AddNavMeshData(navMeshData);
//            Debug.Log("NavMesh烘焙完成");
//            Debug.Log($"烘焙源数量: {filteredSources.Count}");
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