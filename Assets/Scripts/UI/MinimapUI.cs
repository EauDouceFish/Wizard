using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class MinimapUI : UIPanelBase
{
    [SerializeField] GameObject minimapButton; // С��ͼ��ť
    [SerializeField] GameObject minimapPanel;
    [SerializeField] GameObject minimapHexContainer;
    [SerializeField] MinimapHex minimapHexPrefab;

    MapModel mapModel;
    HexGrid hexGrid;
    Storage storage;

    // С��ͼ�����εĴ洢
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

    // ��̬ͼ�����Boss��Element�Ȳ���ı��ͼ�꣩
    Dictionary<Pos2D, IconTypePriority> staticIconTypes = new();
    Dictionary<Pos2D, Sprite> staticIconSprites = new();

    // ��ҵ�ǰλ��
    Pos2D currentPlayerPos;

    #region ��������

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

    #region �¼�����

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
        // �������λ�ò�ˢ��ͼ��
        UpdatePlayerPosition(evt.hexCell.coord);
    }

    //���˴��е�ְ����࣬���������꿼������System��
    // ״̬�ı䣬���Ŀ��HexCell״̬�����ɣ�������ΪMinimapHex���Դ���
    private void OnRegionStateChanged(OnRegionStateChangedEvent evt)
    {
        if (evt.newState == HexCellState.AllCompleted)
        {
            HexCell hexCell = evt.hexCell;
            // ���ö�Ӧ��MinimapHexΪ�ɴ���״̬
            if (minimapHexDict.TryGetValue(hexCell.coord, out MinimapHex minimapHex))
            {
                minimapHex.SetTeleportable();
                minimapHex.SetCompleteColor();
            }
        }
    }

    #endregion

    #region minimap��ʼ������

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

    #region С��ͼ����λ�ü��㡢����
    // �����ͼ����������
    private void CalculateMapCenter()
    {
        if (hexGrid.allHexCellCoordDict.Count == 0)
        {
            mapCenter = Vector2.zero;
            return;
        }

        // �ҵ�������ռ�������ε����귶Χ��Ȼ��ͬ�������ģ���������ͳһ����
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

        // ������������
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

    // ʵ����С��ͼ�����Σ�����С��ͼ�е�λ�á������ɫ�ȵȲ���
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
        // ʹ����HexGrid��ͬ�������������߼�
        Vector2 position;
        position.x = coord.x * minimapHexOuterRadius * 1.5f;
        position.y = coord.y * minimapHexInnerRadius * 2f;

        if (coord.x % 2 == 1)
        {
            position.y -= minimapHexInnerRadius;
        }

        // ��ȥ��ͼ���ĵ�ƫ�ƣ�ʹ��ͼ���Ķ�ӦС��ͼ�������
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

    #region ��ɫ��ͼ�����

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
        // ����HexCell������������ɫ
        if (hexCell.HexRealm != null)
        {
            Color realmColor = hexCell.HexRealm.GetRealmBiome().Color;
            minimapHex.SetColorOpaque(realmColor);
        }
    }

    private void InitIcons()
    {
        // ����Boss�����ͼ��
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

        // ����Ԫ����ͼ��
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

    // ֻ�е�ǰλ�ò������λ��ʱ����������ʾ��̬ͼ��
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


    // ����λ���Ƿ��о�̬ͼ�꣨Ԫ�����ӡ�Boss����Ȼ���Իָ�
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

    #region �����ӿ�

    public void ShowMinimap()
    {
        // �����δ��ʼ���ҵ�ͼ�����ɣ����ʼ��
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
    /// ����HexCell���꣬Ϊָ�������С��ͼHexagon�����ڲ�ͼ��
    /// </summary>
    public void SetMinimapHexIcon(Pos2D coord, Sprite sprite)
    {
        if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
        {
            minimapHex.SetInfoSprite(sprite);
        }
    }

    /// <summary>
    /// Ϊָ�������С��ͼ������������ɫ
    /// </summary>
    public void SetMinimapHexColor(Pos2D coord, Color color)
    {
        if (minimapHexDict.TryGetValue(coord, out MinimapHex minimapHex))
        {
            minimapHex.SetColorOpaque(color);
        }
    }

    /// <summary>
    /// ����HexCell���¶�Ӧ��С��ͼ������
    /// </summary>
    public void UpdateMinimapHexByCell(HexCell hexCell)
    {
        if (minimapHexDict.TryGetValue(hexCell.coord, out MinimapHex minimapHex))
        {
            SetMinimapHexColor(hexCell, minimapHex);
        }
    }

    /// <summary>
    /// �������λ���ⲿ�ӿ�
    /// </summary>
    public void UpdatePlayerIcon(Pos2D playerPos)
    {
        UpdatePlayerPosition(playerPos);
    }

    #endregion
}

// Minimap����������չΪSystem
public struct GetMiniMapEvent
{

}