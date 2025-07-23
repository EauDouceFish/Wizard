using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class HexCellPathFinder : MonoBehaviour, IController
{
    MapGenerationSystem mapGenerationSystem;
    MapModel mapModel;  
    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();
    }

    /// <summary>
    /// A*办法，找到两个HexCell之间的路径
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public Path FindPath(HexCell origin, HexCell destination)
    {
        // 可选/不可选节点
        List<HexCell> openSet = new List<HexCell>();
        HashSet<HexCell> closedSet = new HashSet<HexCell>();

        // 初始化结点进入openSet
        openSet.Add(origin);
        origin.costFromOrigin = 0;

        float tileDistance = origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2;

        while (openSet.Count > 0)
        {
            openSet.Sort((x, y) => x.TotalCost.CompareTo(y.TotalCost));
            HexCell currentHexCell = openSet[0];

            openSet.Remove(currentHexCell);
            closedSet.Add(currentHexCell);

            if (currentHexCell == destination)
            {
                return GetPathBetween(destination, origin);
            }

            // A*遍历当前结点的所有邻居结点
            foreach (HexCell neighbor in GetNeighborHexCells(currentHexCell))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float costToNeighbor = currentHexCell.costFromOrigin + neighbor.TerrainCost + tileDistance;
                if (costToNeighbor < neighbor.costFromOrigin || !openSet.Contains(neighbor))
                {
                    neighbor.costFromOrigin = costToNeighbor;
                    neighbor.costToDestination = Vector3.Distance(destination.transform.position, neighbor.transform.position);
                    neighbor.pathParent = currentHexCell;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 获取一个HexCell的所有邻居HexCell
    /// </summary>
    /// <param name="currentHexCell"></param>
    /// <returns></returns>
    public List<HexCell> GetNeighborHexCells(HexCell currentHexCell)
    {
        List<HexCell> neighbours = new List<HexCell>();
        List<Pos2D> neighbourCoords = HexMetrics.GetHexNeighbourCoords(currentHexCell);
        var cellCoordDict = mapModel.HexGrid.allHexCellCoordDict;

        // 遍历邻居坐标，如果在地图内就加入
        foreach (Pos2D coord in neighbourCoords)
        {
            if (cellCoordDict.ContainsKey(coord))
            {
                neighbours.Add(cellCoordDict[coord]);
            }
        }
        return neighbours;
    }

    // 负责从FindPath建立的父节点关系中，构建路径
    private Path GetPathBetween(HexCell destination, HexCell origin)
    {
        List<HexCell> hexCells = new List<HexCell>();
        HexCell current = destination;

        while (current != origin)
        {
            hexCells.Add(current);
            if (current.pathParent != null)
            {
                current = current.pathParent;
            }
            else
            {
                break;
            }
        }

        hexCells.Add(origin);
        hexCells.Reverse();

        Path path = new()
        {
            hexCells = hexCells.ToArray()
        };

        return path;
    }

    #region Architecture
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
    #endregion
}
