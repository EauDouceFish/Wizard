using UnityEngine;

[CreateAssetMenu(fileName = "DefaultDecorationDistributionConfig", menuName = "MapGenerationSystem/DecorationDistributionConfig")]
public class DecorationDistributionConfig : ScriptableObject
{
    [Header("茂密区域和稀疏区域数量配置")]
    [Range(0, 10)] public int minDenseRegionCount = 0;  // 茂密区域数量 至少
    [Range(0, 10)] public int maxDenseRegionCount = 1;  // 茂密区域数量 至多
    [Range(0, 10)] public int minSparseRegionCount = 0; // 稀疏区域数量 至少
    [Range(0, 10)] public int maxSparseRegionCount = 3; // 稀疏区域数量 至多

    [Header("一级装饰物配置")]
    [Range(0, 6)] public int minLevel1DecorationCount_Dense = 2; // 茂密区域一级装饰物数量 至少
    [Range(0, 6)] public int maxLevel1DecorationCount_Dense = 4; // 茂密区域一级装饰物数量 至多
    [Range(0, 6)] public int minLevel1DecorationCount_Sparse = 1; // 稀疏区域一级装饰物数量 至少
    [Range(0, 6)] public int maxLevel1DecorationCount_Sparse = 2; // 稀疏区域一级装饰物数量 至多

    [Header("二级装饰物配置")]
    [Range(0, 6)] public int minLevel2DecorationCount_Dense = 2;    // 茂密区域二级装饰物数量 至少
    [Range(0, 6)] public int maxLevel2DecorationCount_Dense = 7;    // 茂密区域二级装饰物数量 至多
    [Range(0, 6)] public int minLevel2DecorationCount_Sparse = 2;   // 稀疏区域二级装饰物数量 至少
    [Range(0, 6)] public int maxLevel2DecorationCount_Sparse = 4;   // 稀疏区域二级装饰物数量 至多

    [Header("向内偏移量")]
    [Range(0, 20)] public float minOffsetLevel1 = 4f;   // 一级装饰物向内偏移量 至少
    [Range(0, 20)] public float maxOffsetLevel1 = 8f;   // 一级装饰物向内偏移量 至多

    [Range(0, 50)] public float minOffsetLevel2 = 4f;   // 二级装饰物向内偏移量 至少
    [Range(0, 50)] public float maxOffsetLevel2 = 10f; // 二级装饰物向内偏移量 至多
}
