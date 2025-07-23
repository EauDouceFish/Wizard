using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// ���п���Buff��ʵ�壬��Ҫά��һ��Buff�б�����ʵ�����׷���  
/// </summary>  
public interface IBuffableEntity
{
    BuffSystem<Entity> BuffSystem { get; }
    HashSet<EntityBuffBase> Buffs { get; }
    void AddBuff(EntityBuffBase buff);
    void RemoveBuff(EntityBuffBase buff);
}