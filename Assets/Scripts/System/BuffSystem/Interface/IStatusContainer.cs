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
/// �������Ԫ��״̬������ӿڣ���Ҫά��һ��Ԫ��״̬��ʵ����ط����������ⲿ���м���
/// </summary>
public interface ICanAddElementStatus
{
    // �洢��ǰӵ�е�Ԫ��״̬
    HashSet<ElementStatusType> StatusTypes { get; }

    protected void AddStatus(ElementStatusType statusType);

    protected void RemoveStatus(ElementStatusType statusType);
}

