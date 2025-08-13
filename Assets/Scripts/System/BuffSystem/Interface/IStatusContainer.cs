using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public enum ElementStatusType
{
    Fire,
    Water,
    Ice,
    Nature,
    Rock,
}


/// <summary>
/// 可以添加元素状态的物体接口，需要维护一个元素状态，实现相关方法，用来外部进行计算
/// </summary>
public interface ICanAddElementStatus
{
    // 存储当前拥有的元素状态
    HashSet<ElementStatusType> StatusTypes { get; }

    protected void AddStatus(ElementStatusType statusType);

    protected void RemoveStatus(ElementStatusType statusType);
}

