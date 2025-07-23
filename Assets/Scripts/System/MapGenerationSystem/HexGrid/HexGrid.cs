using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using System;

/// <summary>
/// �����������ͼ���࣬������ͼ������Mesh�򻯡�Ѱ·���ܵ�
/// </summary>
public class HexGrid : MonoBehaviour, IController
{
    private MapGenerationSystem mapGenerationSystem;
    private MapModel mapModel;
    private GameObject hexCellsCollector;
    private HexCellPathFinder hexCellPathFinder;                    // ������CellѰ·��
    private HexMesh hexMesh;
    private EdgeFillConfig edgeFillConfig;                          // ��Ե����ϰ�������

    [SerializeField] private HexCell cellPrefab;                    // ������CellԤ����

    public Dictionary<Pos2D, HexCell> allHexCellCoordDict = new();  // �洢������HexCell���Լ���������ӳ��

    private List<HexRealm> hexRealms = new List<HexRealm>();        // �����������б�

    private HexCell[] cells;
    private float outerRadius;
    private float innerRadius;
    
    #region ��������

    private void Awake()
    {
        mapGenerationSystem = this.GetSystem<MapGenerationSystem>();
        mapModel = mapGenerationSystem.GetMapModel();
        var storage = this.GetUtility<Storage>();

        // �洢�����������ͼ���ݡ�Ⱥϵ��������
        mapModel.SetHexGrid(this);
        mapModel.SetBiomeConfigData(storage.GetAllBiomeSO());

        // ����HexGrid��������һ������
        hexMesh = this.GetOrAddComponent<HexMesh>();
        InitHexCellPathFinder();
    }

    private void Start()
    {
        // �������
        outerRadius = mapModel.HexOuterRadius;
        innerRadius = mapModel.HexInnerRadius;

        CreateHexCollector();
    }
    #endregion

    #region �ⲿ���Է���

    /// <summary>
    /// ����Ҫ��ʼ����HexCell�������С��֮����м��㲢��������������
    /// </summary>
    public void CreateHexGridXZ(int width = 16, int height = 8)
    {
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCellXZ(x, z, i++);
            }
        }

        hexMesh.Triangulate(cells);
    }

    public HexCell GetCellByCoord(Pos2D coord)
    {
        if (allHexCellCoordDict.TryGetValue(coord, out HexCell cell))
        {
            return cell;
        }
        return null;
    }

    public HexCell GetCellByCoord(int x, int z)
    {
        Pos2D coord = new(x, z);
        if (allHexCellCoordDict.TryGetValue(coord, out HexCell cell))
        {
            return cell;
        }
        return null;
    }

    /// <summary>
    /// ��ȡ���������HexCell
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public HexCell[] GetRandomCells(int count)
    {
        if (allHexCellCoordDict.Count == 0)
        {
            return new HexCell[0];
        }

        int actualCount = Mathf.Min(count, allHexCellCoordDict.Count);

        // ��ȡ����HexCell�ȸ�����ѡ
        List<HexCell> allCells = new List<HexCell>(allHexCellCoordDict.Values);

        HexCell[] randomCells = new HexCell[actualCount];

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allCells.Count);
            randomCells[i] = allCells[randomIndex];
            allCells.RemoveAt(randomIndex);
        }

        return randomCells;
    }

    /// <summary>
    /// �������ͼ�ڴ���һ������Ⱥϵ��Realm
    /// </summary>
    /// <param name="initHexCell"></param>
    /// <param name="biome"></param>
    public void CreateHexRealm(HexCell initHexCell, Biome biome)
    {
        HexRealm hexRealm = new HexRealm(initHexCell, biome);
        hexRealms.Add(hexRealm);
        // �˴��������޸�Ϊ�¼�
        mapModel.AddBiome(biome);
    }

    public List<HexRealm> GetHexRealms()
    {
        return hexRealms;
    }

    public HexCellPathFinder GetHexCellPathFinder()
    {
        return hexCellPathFinder;
    }


    #endregion

    #region ˽���߼�����

    /// <summary>
    /// ����������Cell���ռ���
    /// </summary>
    private void CreateHexCollector()
    {
        if (hexCellsCollector == null)
        {
            hexCellsCollector = new GameObject("HexCellsCollector");
            hexCellsCollector.transform.position = Vector3.zero;
            hexCellsCollector.transform.SetParent(this.transform, false);
        }
    }

    /// <summary>
    /// ���쵥��������Cell
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="index"></param>
    private void CreateCellXZ(int x, int z, int index)
    {
        Vector3 position = CalculateNewCenterXZ(x, z);
        Pos2D coord = new(x, z);
        HexCell cell = cells[index] = Instantiate(cellPrefab);
        cell.transform.SetParent(hexCellsCollector.transform, false);

        // ��ʼʱ�����������Ժ�ͨ�� HexMesh ������
        cell.InitializeXZ(position, coord);
        if (allHexCellCoordDict.ContainsKey(coord))
        {
            Debug.LogWarning($"index: {index}, ({coord.x},{coord.y}) �Ѿ�����");
        }
        else
        allHexCellCoordDict.Add(coord, cell);
    }

    /// <summary>
    /// ��3D�ռ�XZƽ����������������
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private Vector3 CalculateNewCenterXZ(int x, int z)
    {
        Vector3 position;
        position.x = x * outerRadius * 1.5f;
        position.y = 0f;
        position.z = z * innerRadius * 2f;

        // �����к���ƫ�ư����뾶���Զ�������
        if (x % 2 == 1)
        {
            position.z -= innerRadius;
        }
        return position;
    }

    private void InitHexCellPathFinder()
    {
        GameObject hexCellPathFinder = new GameObject("HexCellPathFinder");
        hexCellPathFinder.transform.SetParent(transform);
        hexCellPathFinder.transform.localPosition = Vector3.zero;
        this.hexCellPathFinder = hexCellPathFinder.AddComponent<HexCellPathFinder>();
    }

    #endregion

    #region Interfaces

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
