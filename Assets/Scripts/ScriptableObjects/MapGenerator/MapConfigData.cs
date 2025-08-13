using UnityEngine;

/// <summary>
/// 基本的Map配置数据
/// </summary>
[CreateAssetMenu(fileName = "NewMapConfigData", menuName = "MapGenerationSystem/MapConfigData")]
public class MapConfigData : ScriptableObject
{
    public string configName = "DefaultMapConfig";      // 配置名称

    // 俯视角下，默认Map的宽和高
    public int MiniMapWidth = 700;                      
    public int MiniMapHeight = 500;
    public int SmallMapWidth = 1000;
    public int SmallMapHeight = 1000;
    public int MediumMapWidth = 1200;
    public int MediumMapHeight = 1200;
    public int LargeMapWidth = 1500;
    public int LargeMapHeight = 1500;

    // Map最大支持的群系数量
    public int MaxBoimeSupportMini = 2;
    public int MaxBoimeSupportSmall = 3;
    public int MaxBoimeSupportMedium = 4;
    public int MaxBoimeSupportLarge = 5;

    [Header("地图描述配置")]
    [TextArea(3, 5)]
    public string miniMapDescription = "一个仅有{0}种群系、极小的地图，适合快速简单体验";
    [TextArea(3, 5)]
    public string smallMapDescription = "包含{0}种群系小型地图。因此元素魔法的组合也受限";
    [TextArea(3, 5)]
    public string mediumMapDescription = "包含{0}种群系的正规地图，开始游戏！";
    [TextArea(3, 5)]
    public string largeMapDescription = "包含{0}种群系，有非常多区域的大地图，总体游戏时间长";
}
