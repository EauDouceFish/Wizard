using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 六边形领域类
/// </summary>
public class HexRealm : ICanGetModel
{
    private MapModel m_mapModel;
    private HexGrid m_hexGrid;

    // 领域初始的HexCell
    private HexCell initHexCell;
    // 领域所拥有的群系环境
    private Biome realmBiome;
    Color realmCellColor => realmBiome != null ? realmBiome.Color : Color.white;

    /// <summary>
    /// 保存领域内的所有HexCell，可用坐标查找
    /// </summary>
    Dictionary<Pos2D, HexCell> realmHexCellDict = new();

    //List<CircleItem> circleItemsInRealm = new List<CircleItem>();

    /// <summary>
    /// 实例化一个HexRealm并且传入初始群系信息
    /// </summary>
    /// <param name="hexCell"></param>
    public HexRealm(HexCell hexCell, Biome biome = null)
    {
        initHexCell = hexCell;
        realmBiome = biome;
        AddHexCellIntoRealm(initHexCell);
    }

    /// <summary>
    /// 纳入单个HexCell扩充领域的方法：
    /// </summary>
    public void AddHexCellIntoRealm(HexCell targetCell)
    {
        // 纳入无主的Cell进入领域
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
    /// 返回领域初始位置中心表面的坐标
    /// </summary>
    public Vector3 GetRealmCenterUponGround()
    {
        return GOExtensions.GetGroundPosition(initHexCell.transform.position);
    }

    /// <summary>
    /// 重新设置领域的中心HexCell
    /// </summary>
    public void ResetRealmCenter(HexCell newCenter)
    {
        // 忽略不属于的HexCell
        if (!realmHexCellDict.ContainsKey(newCenter.coord)) return;

        initHexCell = newCenter;
    }

    /// <summary>
    /// 添加 CircleItem 进入领域
    /// </summary>
    //public void AddCircleItemIntoRealm(CircleItem circleItem)
    //{
    //    circleItem.SetCircleItemColor(hexRealmColor);
    //    circleItemsInRealm.Add(circleItem);
    //}

    ///// <summary>
    ///// 计算CircleItem 和 initHexCell 的距离，越近排序越靠前
    ///// </summary>
    //public void SortCircleItemInRealmByPosition()
    //{
    //    circleItemsInRealm.Sort((item1, item2) =>
    //    {
    //        float distance1 = Vector3.Distance(item1.transform.position, initHexCell.transform.position);
    //        float distance2 = Vector3.Distance(item2.transform.position, initHexCell.transform.position);

    //        // 按距离升序排序（越近越前）
    //        return distance1.CompareTo(distance2);
    //    });
    //}

    //// 生成领域内的一级区域
    //public void SummonFirstRegion()
    //{
    //    // 暂时生成主城
    //    if (circleItemsInRealm.Count > 5)
    //    {
    //        int randomIndex = Random.Range(0, 5);
    //        CircleItem targetCircle = circleItemsInRealm[randomIndex];

    //    }
    //}

    #region 成员变量配套方法

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
    /// 获取领域内的大小
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
    /// 获取领域内所有HexCell  
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
