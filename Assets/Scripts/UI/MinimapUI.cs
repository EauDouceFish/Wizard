using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class MinimapUI : UIPanelBase
{
    [SerializeField] GameObject minimapButton; // 小地图按钮
    [SerializeField] GameObject minimapPanel;
    [SerializeField] GameObject minimapHexContainer;
    [SerializeField] MinimapHex minimapHexPrefab;

    MapModel mapModel;
    HexGrid hexGrid;
    Storage storage;

    // 小地图六边形的存储
    Dictionary<Pos2D, MinimapHex> minimapHexDict = new();

    float minimapHexOuterRadius;
    float minimapHexInnerRadius;
    Vector2 mapCenter;

    public enum IconTypePriority
    {
        None = 0,
        Player = 1,
        Element = 2,
        Boss = 3,
    }

    // 静态图标管理（Boss、Element等不会改变的图标）
    Dictionary<Pos2D, IconTypePriority> staticIconTypes = new();
    Dictionary<Pos2D, Sprite> staticIconSprites = new();

    // 玩家当前位置
    Pos2D currentPlayerPos;

    #region 生命周期

    protected override void Awake()
    {
        base.Awake();
        mapModel = this.GetModel<MapModel>();
        storage = this.GetUtility<Storage>();

        this.RegisterEvent<GetMiniMapEvent>(OnMinimapGet).UnRegisterWhenCurrentSceneUnloaded();

        this.RegisterEvent<MapGenerationCompletedEvent>(OnMapGenerationCompleted)
            .UnRegisterWhenCurrentSceneUnloaded();

        this.RegisterEvent<OnRegionEnterTriggerEvent>(OnRegionEnterTriggered)
            .UnRegisterWhenCurrentSceneUnloaded();

        this.RegisterEvent<OnRegionStateChangedEvent>(OnRegionStateChanged)
            .UnRegisterWhenCurrentSceneUnloaded();
    }

    protected override void Start()
    {
        base.Start();
    }

    #endregion

    #region 事件处理

    private void OnMinimapGet(GetMiniMapEvent evt)
    {
        minimapButton.SetActive(true);
    }

    private void OnMapGenerationCompleted(MapGenerationCompletedEvent evt)
    {
        hexGrid = mapModel.HexGrid;
        InitializeMinimap();
    }

    private void OnRegionEnterTriggered(OnRegionEnterTriggerEvent evt)
    {
        // 更新玩家位置并刷新图标
        UpdatePlayerPosition(evt.hexCell.coord);
    }

    //【此处承担职责过多，后续开发完考虑移入System】
    // 状态改变，如果目标HexCell状态变成完成，则设置为MinimapHex可以传送
    private void OnRegionStateChanged(OnRegionStateChangedEvent evt)
    {
        if (evt.newState == HexCellState.AllCompleted)
        {
            HexCell hexCell = evt.hexCell;
            // 设置对应的MinimapHex为可传送状态
            if (minimapHexDict.TryGetValue(hexCell.coord, out MinimapHex minimapHex))
            {
                minimapHex.SetTeleportable();
                minimapHex.SetCompleteColor();
            }
        }
    }

    #endregion

    #region minimap初始化生成

    private void InitializeMinimap()
    {
        minimapHexOuterRadius = minimapHexPrefab.GetHexOuterRadius();
        minimapHexInnerRadius = minimapHexOuterRadius * 0.866025404f; // sqrt(3)/2

        CalculateMapCenter();

        ClearExistingMinimapHexes();

        CreateMinimapHexes();

        UpdateMinimapColors();

        InitIcons();
    }

    #region 小地图格子位置计算、生成
    // 计算地图的中心坐标
    private void CalculateMapCenter()
    {
        if (hexGrid.allHexCellCoordDict.Count == 0)
        {
            mapCenter = Vector2.zero;
            return;
        }

        // 找到所有已占用六边形的坐标范围，然后同步到中心，这样才能统一计算
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var kvp in hexGrid.allHexCellCoordDict)
        {
            if (kvp.Value.isOccupied)
            {
                Pos2D coord = kvp.Key;
                minX = Mathf.Min(minX, coord.x);
                maxX = Mathf.Max(maxX, coord.x);
                minY = Mathf.Min(minY, coord.y);
                maxY = Mathf.Max(maxY, coord.y);
            }
        }

        // 计算中心坐标
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        mapCenter = new Vector2(centerX, centerY);
    }

    private void ClearExistingMinimapHexes()
    {
        foreach (Transform child in minimapHexContainer.transform)
        {
            Destroy(child.gameObject);
        }
        minimapHexDict.Clear();
        staticIconTypes.Clear();
        staticIconSprites.Clear();
    }

    private void CreateMinimapHexes()
    {
        foreach (var kvp in hexGrid.allHexCellCoordDict)
        {
            HexCell hexCell = kvp.Value;
            if (hexCell.isOccupied)
            {
                CreateMinimapHex(hexCell);
            }
        }
    }

    // 实例化小地图六边形，计算小地图中的位置、填充颜色等等操作
    private void CreateMinimapHex(HexCell hexCell)
    {
        MinimapHex minimapHex = Instantiate(minimapHexPrefab, minimapHexContainer.transform);

        Vector2 minimapPosition = CalculateMinimapPosition(hexCell.coord);
        RectTransform rect = minimapHex.GetComponent<RectTransform>();
        rect.anchoredPosition = minimapPosition;

        minimapHex.SetRepresentingHexCell(hexCell);
        minimapHexDict[hexCell.coord] = minimapHex;

        SetMinimapHexColor(hexCell, minimapHex);
    }

    private Vector2 CalculateMinimapPosition(Pos2D coord)
    {
        // 使用与HexGrid相同的六边形排列逻辑
        Vector2 position;
        position.x = coord.x * minimapHexOuterRadius * 1.5f;
        position.y = coord.y * minimapHexInnerRadius * 2f;

        if (coord.x % 2 == 1)
        {
            position.y -= minimapHexInnerRadius;
        }

        // 减去地图中心的偏移，使地图中心对应小地图面板中心
        Vector2 centerOffset;
        centerOffset.x = mapCenter.x * minimapHexOuterRadius * 1.5f;
        centerOffset.y = mapCenter.y * minimapHexInnerRadius * 2f;

        if (Mathf.RoundToInt(mapCenter.x) % 2 == 1)
        {
            centerOffset.y -= minimapHexInnerRadius;
        }

        position -= centerOffset;

        return position;
    }

    #endregion

    #endregion

    #region 颜色和图标更新

    private void UpdateMinimapColors()
    {
        foreach (var kvp in minimapHexDict)
        {
            Pos2D coord = kvp.Key;
            MinimapHex minimapHex = kvp.Value;

            HexCell hexCell = hexGrid.GetCellByCoord(coord);
            if (hexCell != null)
            {
                SetMinimapHexColor(hexCell, minimapHex);
            }
        }
    }

    private void SetMinimapHexColor(HexCell hexCell, MinimapHex minimapHex)
    {
        // 根据HexCell的领域设置颜色
        if (hexCell.HexRealm != null)
        {
            Color realmColor = hexCell.HexRealm.GetRealmBiome().Color;
            minimapHex.SetColorOpaque(realmColor);
        }
    }

    private void InitIcons()
    {
        // 设置Boss、玩家图标
        foreach (var kvp in hexGrid.allHexCellCoordDict)
        {
            HexCell hexCell = kvp.Value;
            if (hexCell.isEndLocation)
            {
                SetStaticIcon(hexCell.coord, IconTypePriority.Boss, storage.GetBossIcon());
            }
            if (hexCell.isPlayerSpawn)
            {
                UpdatePlayerPosition(kvp.Value.coord);
                break;
            }
        }

        // 设置元素柱图标
        foreach (var realm in hexGrid.GetHexRealms())
        {
            HexCell centerCell = realm.GetInitHexCell();
            MagicElement element = realm.GetRealmBiome().MagicElement;
            SetStaticIcon(centerCell.coord, IconTypePriority.Element, storage.GetElementIcon(element));
        }
    }

    private void UpdatePlayerPosition(Pos2D newPlayerPos)
    {
        if (currentPlayerPos != null)
        {
            RestoreStaticIconIfExists(currentPlayerPos);
        }

        currentPlayerPos = newPlayerPos;
        SetPlayerIcon(newPlayerPos);
    }

    // 只有当前位置不是玩家位置时，才立即显示静态图标
    private void SetStaticIcon(Pos2D coord, IconTypePriority iconType, Sprite sprite)
    {
        staticIconTypes[coord] = iconType;
        staticIconSprites[coord] = sprite;

        if (coord != currentPlayerPos)
        {
            if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
            {
                minimapHex.SetInfoSprite(sprite);
            }
        }
    }


    private void SetPlayerIcon(Pos2D coord)
    {
        if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
        {
            minimapHex.SetInfoSprite(storage.GetPlayerIcon());
        }
    }


    // 检查该位置是否有静态图标（元素柱子、Boss），然后尝试恢复
    private void RestoreStaticIconIfExists(Pos2D coord)
    {
        if (staticIconTypes.TryGetValue(coord, out IconTypePriority staticType) && staticIconSprites.TryGetValue(coord, out Sprite staticSprite))
        {
            if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
            {
                minimapHex.SetInfoSprite(staticSprite);
            }
        }
        else
        {
            if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
            {
                minimapHex.SetInfoSprite(null);
            }
        }
    }

    #endregion

    #region 公共接口

    public void ShowMinimap()
    {
        // 如果还未初始化且地图已生成，则初始化
        if (minimapHexDict.Count == 0)
        {
            InitializeMinimap();
        }
        minimapPanel.SetActive(true);
    }

    public void HideMinimap()
    {
        minimapPanel.SetActive(false);
    }


    /// <summary>
    /// 传入HexCell坐标，为指定坐标的小地图Hexagon设置内部图标
    /// </summary>
    public void SetMinimapHexIcon(Pos2D coord, Sprite sprite)
    {
        if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
        {
            minimapHex.SetInfoSprite(sprite);
        }
    }

    /// <summary>
    /// 为指定坐标的小地图六边形设置颜色
    /// </summary>
    public void SetMinimapHexColor(Pos2D coord, Color color)
    {
        if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
        {
            minimapHex.SetColorOpaque(color);
        }
    }

    /// <summary>
    /// 根据HexCell更新对应的小地图六边形
    /// </summary>
    public void UpdateMinimapHexByCell(HexCell hexCell)
    {
        if (minimapHexDict.TryGetValue(hexCell.coord, out MinimapHex minimapHex))
        {
            SetMinimapHexColor(hexCell, minimapHex);
        }
    }

    /// <summary>
    /// 更新玩家位置外部接口
    /// </summary>
    public void UpdatePlayerIcon(Pos2D playerPos)
    {
        UpdatePlayerPosition(playerPos);
    }

    #endregion
}

// Minimap后续考虑拓展为System
public struct GetMiniMapEvent
{

}