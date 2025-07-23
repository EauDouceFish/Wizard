using UnityEngine;

[CreateAssetMenu(fileName = "EdgeFillConfig", menuName = "MapGenerationSystem/EdgeFillConfig")]
public class EdgeFillConfig : ScriptableObject
{
    [Header("岩石单次旋转角度、最大累计旋转角度")]
    [Range(0f, 15f)]
    public float singleRotationRange = 5f;             // 单次旋转角度范围
    [Range(0f, 30f)]
    public float cumulativeRotationRange = 15f;        // 累计旋转偏移范围

    [Header("期望最大填充地块数配置")]
    [Range(1, 10)]
    public int maxFillCount = 3;                       // 期望最大边填充地块数

    [Header("拉伸比例，最大拉伸比例需要大于最小比例")]
    [Range(0.1f, 2f)]
    public float minStretchRatio = 0.5f;               // 最小拉伸比例
    [Range(0.5f, 3f)]
    public float maxStretchRatio = 1.5f;               // 最大拉伸比例

    [Header("中线连接处，向内的偏移量")]
    public float innerOffset = 0.2f;                   // 中线连接处向内偏移量

    [Header("末尾段最小修正量")]
    public float minStretchCorrection = 0.3f;              // 末尾段最小修正量
}