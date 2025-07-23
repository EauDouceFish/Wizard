using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 所有可以Buff的实体，需要维护一个Buff列表，并且实现配套方法  
/// </summary>  
public interface IBuffableEntity
{
    BuffSystem<Entity> BuffSystem { get; }
    HashSet<EntityBuffBase> Buffs { get; }
    void AddBuff(EntityBuffBase buff);
    void RemoveBuff(EntityBuffBase buff);
}