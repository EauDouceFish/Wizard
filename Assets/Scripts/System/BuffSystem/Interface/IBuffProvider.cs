using QFramework;
using UnityEngine;

public interface IBuffProvider
{
    /// <summary>
    /// 获取提供者名称
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// 向实体添加Buff
    /// </summary>
    /// <param name="entity">目标实体</param>
    void ApplyBuffToEntity(IBuffableEntity entity);
}