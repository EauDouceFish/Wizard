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
    /// A*�취���ҵ�����HexCell֮���·��
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public Path FindPath(HexCell origin, HexCell destination)
    {
        // ��ѡ/����ѡ�ڵ�
        List<HexCell> openSet = new List<HexCell>();
        HashSet<HexCell> closedSet = new HashSet<HexCell>();

        // ��ʼ��������openSet
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

            // A*������ǰ���������ھӽ��
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
    /// ��ȡһ��HexCell�������ھ�HexCell
    /// </summary>
    /// <param name="currentHexCell"></param>
    /// <returns></returns>
    public List<HexCell> GetNeighborHexCells(HexCell currentHexCell)
    {
        List<HexCell> neighbours = new List<HexCell>();
        List<Pos2D> neighbourCoords = HexMetrics.GetHexNeighbourCoords(currentHexCell);
        var cellCoordDict = mapModel.HexGrid.allHexCellCoordDict;

        // �����ھ����꣬����ڵ�ͼ�ھͼ���
        foreach (Pos2D coord in neighbourCoords)
        {
            if (cellCoordDict.ContainsKey(coord))
            {
                neighbours.Add(cellCoordDict[coord]);
            }
        }
        return neighbours;
    }

    // �����FindPath�����ĸ��ڵ��ϵ�У�����·��
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

    /// <summary>
    /// ��ȡ�����ھ�������������Զ��HexCell�������ض�λPlayer�ĳ����㣬�Լ������յ�λ��
    /// </summary>
    /// <param name="hexRealm">Ҫ���ҵ�����</param>
    /// <returns>����������Զ��HexCell���������Ϊ�շ���null</returns>
    public HexCell GetFarthestCellFromRealmCenter(HexRealm hexRealm)
    {
        List<HexCell> cellsInRealm = hexRealm.GetHexCellsInRealm();

        Vector3 realmCenter = hexRealm.GetRealmCenterUponGround();
        HexCell farthestCell = null;
        float maxDistance = 0f;

        foreach (HexCell cell in cellsInRealm)
        {
            float distance = Vector3.Distance(cell.transform.position, realmCenter);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestCell = cell;
            }
        }

        return farthestCell;
    }


    #region Architecture
    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
    #endregion
}
