using UnityEngine;

/// <summary>
/// 地图配置管理器，用于在架构重新初始化时保持地图设置
/// </summary>
public static class MapConfigurationManager
{
    private static MapSize selectedMapSize = MapSize.Default;

    public static void SetSelectedMapSize(MapSize mapSize)
    {
        selectedMapSize = mapSize;
        Debug.Log($"MapConfigurationManager: 地图大小设置为 {mapSize}");
    }

    public static MapSize GetSelectedMapSize()
    {
        return selectedMapSize;
    }

    public static bool HasValidMapSize()
    {
        return selectedMapSize != MapSize.Default;
    }

    public static void Reset()
    {
        selectedMapSize = MapSize.Default;
        Debug.Log("MapConfigurationManager: 地图配置已重置");
    }
}