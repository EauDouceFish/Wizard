using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// ���п���Buff��ʵ�壬��Ҫά��һ��Buff�б�����ʵ�����׷���  
/// </summary>  
public interface IBuffableEntity
{
    /// <summary>
    /// ʵ����������Buff�Ĵ���ϵͳ����Ҫ��ʼ��
    /// </summary>
    BuffSystem<Entity> BuffSystem { get; }
}