using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;

public class MapSelectDescUI : UIPanelBase
{
    [SerializeField] TextMeshProUGUI biomeCountText;
    [SerializeField] TextMeshProUGUI mapSizeText;
    [SerializeField] TextMeshProUGUI mapDesc;

    private Storage storage;
    private MapConfigData mapConfigData;

    protected override void Awake()
    {
        base.Awake();
        storage = this.GetUtility<Storage>();
        mapConfigData = storage.GetDefaultMapConfigData();
    }

    public void UpdateMapDescription(MapSize mapSize)
    {
        if (mapConfigData == null) return;

        int biomeCount = GetBiomeCountBySize(mapSize);
        Vector2Int mapDimensions = GetMapDimensionsBySize(mapSize);
        string description = GetFormattedMapDescription(mapSize);

        if (biomeCountText != null)
            biomeCountText.text = $"包含随机群系数量：{biomeCount}";

        if (mapSizeText != null)
            mapSizeText.text = $"地图规格：{mapDimensions.x}x{mapDimensions.y}";

        if (mapDesc != null) mapDesc.text = description;
    }

    private int GetBiomeCountBySize(MapSize size)
    {
        return size switch
        {
            MapSize.Mini => mapConfigData.MaxBoimeSupportMini,
            MapSize.Small => mapConfigData.MaxBoimeSupportSmall,
            MapSize.Medium => mapConfigData.MaxBoimeSupportMedium,
            MapSize.Large => mapConfigData.MaxBoimeSupportLarge,
            _ => mapConfigData.MaxBoimeSupportMini
        };
    }

    private Vector2Int GetMapDimensionsBySize(MapSize size)
    {
        return size switch
        {
            MapSize.Mini => new Vector2Int(mapConfigData.MiniMapWidth, mapConfigData.MiniMapHeight),
            MapSize.Small => new Vector2Int(mapConfigData.SmallMapWidth, mapConfigData.SmallMapHeight),
            MapSize.Medium => new Vector2Int(mapConfigData.MediumMapWidth, mapConfigData.MediumMapHeight),
            MapSize.Large => new Vector2Int(mapConfigData.LargeMapWidth, mapConfigData.LargeMapHeight),
            _ => new Vector2Int(mapConfigData.MiniMapWidth, mapConfigData.MiniMapHeight)
        };
    }

    public string GetFormattedMapDescription(MapSize mapSize)
    {
        return mapSize switch
        {
            MapSize.Mini => string.Format(mapConfigData.miniMapDescription, mapConfigData.MaxBoimeSupportMini),
            MapSize.Small => string.Format(mapConfigData.smallMapDescription, mapConfigData.MaxBoimeSupportSmall),
            MapSize.Medium => string.Format(mapConfigData.mediumMapDescription, mapConfigData.MaxBoimeSupportMedium),
            MapSize.Large => string.Format(mapConfigData.largeMapDescription, mapConfigData.MaxBoimeSupportLarge),
            _ => string.Format(mapConfigData.miniMapDescription, mapConfigData.MaxBoimeSupportMini)
        };
    }
}