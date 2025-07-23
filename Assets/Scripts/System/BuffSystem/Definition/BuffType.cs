using UnityEngine;
public enum BuffType
{
    /// <summary>
    /// 正面的Buff
    /// </summary>
    [InspectorName("正面Buff")]
    Positive,

    /// <summary>
    /// 负面的Buff
    /// </summary>
    [InspectorName("负面Buff")]
    Negative,

    /// <summary>
    /// 中性Buff
    /// </summary>
    [InspectorName("中性的")]
    Neutral,
}
