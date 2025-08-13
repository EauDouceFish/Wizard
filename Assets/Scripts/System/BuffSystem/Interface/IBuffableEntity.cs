using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 所有可以Buff的实体，需要维护一个Buff列表，并且实现配套方法  
/// </summary>  
public interface IBuffableEntity
{
    /// <summary>
    /// 实体身上所有Buff的处理系统，需要初始化
    /// </summary>
    BuffSystem<Entity> BuffSystem { get; }
}