using QFramework;
using UnityEngine;

public interface IBuffProvider
{
    /// <summary>
    /// ��ȡ�ṩ������
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// ��ʵ�����Buff
    /// </summary>
    /// <param name="entity">Ŀ��ʵ��</param>
    void ApplyBuffToEntity(IBuffableEntity entity);
}