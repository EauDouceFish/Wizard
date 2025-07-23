using UnityEngine;

/// <summary>
/// 当两个同类Buff存在时候的处理方式
/// </summary>
public enum BuffConflictResolution
{
    /// <summary>
    /// 合并为一个buff，叠层
    /// </summary>
    [InspectorName("合并升级")]
    Combine,
    
    /// <summary>
    /// 独立存在，效果翻倍不相互影响
    /// </summary>
    [InspectorName("独立存在")]
    Separate,

    /// <summary>
    /// 后者覆盖前者
    /// </summary>
    [InspectorName("覆盖前者")]
    Cover,
}
