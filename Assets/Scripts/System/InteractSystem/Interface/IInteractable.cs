using UnityEngine;

/// <summary>
/// 要交互的物体要实现此接口，实现ColliderTrigger相关逻辑并且传入
/// 通常使用IInteractableEntity继承此接口
/// </summary>
public interface IInteractable
{
    Collider TriggerCollider { get;}
    GameObject GameObject { get; }
    
    /// <summary>
    /// 靠近时提示信息
    /// </summary>
    string InteractTipText { get; }
    /// <summary>
    /// 交互后的反应文本
    /// </summary>
    string InteractReactionText { get; }
    bool CanInteract { get; }
    public void OnInteract(GameObject player);
}