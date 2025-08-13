using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// ������������
/// </summary>
public class HexRealm : ICanGetModel
{
    private MapModel m_mapModel;
    private HexGrid m_hexGrid;

    // �����ʼ��HexCell
    private HexCell initHexCell;
    // ������ӵ�е�Ⱥϵ����
    private Biome realmBiome;
    Color realmCellColor => realmBiome != null ? realmBiome.Color : Color.white;

    /// <summary>
    /// ���������ڵ�����HexCell�������������
    /// </summary>
    Dictionary<Pos2D, HexCell> realmHexCellDict = new();

    //List<CircleItem> circleItemsInRealm = new List<CircleItem>();

    /// <summary>
    /// ʵ����һ��HexRealm���Ҵ����ʼȺϵ��Ϣ
    /// </summary>
    /// <param name="hexCell"></param>
    public HexRealm(HexCell hexCell, Biome biome = null)
    {
        initHexCell = hexCell;
        realmBiome = biome;
        AddHexCellIntoRealm(initHexCell);
    }

    /// <summary>
    /// ���뵥��HexCell��������ķ�����
    /// </summary>
    public void AddHexCellIntoRealm(HexCell targetCell)
    {
        // ����������Cell��������
        if (!targetCell.isOccupied)
        {
            targetCell.isOccupied = true;
            realmHexCellDict.Add(targetCell.coord, targetCell);
            targetCell.SetColor(realmCellColor);
            targetCell.SetHexRealm(this);
            targetCell.CheckAndBindEdgesWithOccupiedNeighbours();
        }
    }

    /// <summary>
    /// ���������ʼλ�����ı��������
    /// </summary>
    public Vector3 GetRealmCenterUponGround()
    {
        return GOExtensions.GetGroundPosition(initHexCell.transform.position);
    }

    /// <summary>
    /// �����������������HexCell
    /// </summary>
    public void ResetRealmCenter(HexCell newCenter)
    {
        // ���Բ����ڵ�HexCell
        if (!realmHexCellDict.ContainsKey(newCenter.coord)) return;

        initHexCell = newCenter;
    }

    /// <summary>
    /// ��� CircleItem ��������
    /// </summary>
    //public void AddCircleItemIntoRealm(CircleItem circleItem)
    //{
    //    circleItem.SetCircleItemColor(hexRealmColor);
    //    circleItemsInRealm.Add(circleItem);
    //}

    ///// <summary>
    ///// ����CircleItem �� initHexCell �ľ��룬Խ������Խ��ǰ
    ///// </summary>
    //public void SortCircleItemInRealmByPosition()
    //{
    //    circleItemsInRealm.Sort((item1, item2) =>
    //    {
    //        float distance1 = Vector3.Distance(item1.transform.position, initHexCell.transform.position);
    //        float distance2 = Vector3.Distance(item2.transform.position, initHexCell.transform.position);

    //        // ��������������Խ��Խǰ��
    //        return distance1.CompareTo(distance2);
    //    });
    //}

    //// ���������ڵ�һ������
    //public void SummonFirstRegion()
    //{
    //    // ��ʱ��������
    //    if (circleItemsInRealm.Count > 5)
    //    {
    //        int randomIndex = Random.Range(0, 5);
    //        CircleItem targetCircle = circleItemsInRealm[randomIndex];

    //    }
    //}

    #region ��Ա�������׷���

    public HexCell GetInitHexCell()
    {
        return initHexCell;
    }

    //public void SetHexRealmColor(Color newColor)
    //{
    //    hexRealmColor = newColor;
    //}

    //public Color GetHexRealmColor()
    //{
    //    return hexRealmColor;
    //}

    /// <summary>
    /// ��ȡ�����ڵĴ�С
    /// </summary>
    /// <returns></returns>
    public int GetHexRealmSize()
    {
        return realmHexCellDict.Count;
    }

    public Biome GetRealmBiome()
    {
        return realmBiome;
    }

    /// <summary>  
    /// ��ȡ����������HexCell  
    /// </summary>  
    /// <returns></returns>  
    public List<HexCell> GetHexCellsInRealm()
    {
        return new List<HexCell>(realmHexCellDict.Values);
    }

    #endregion

    #region Architecture

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }

    #endregion
}
