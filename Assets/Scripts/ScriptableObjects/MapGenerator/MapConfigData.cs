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
}
