using UnityEngine;

/// <summary>
/// Ҫ����������Ҫʵ�ִ˽ӿڣ�ʵ��ColliderTrigger����߼����Ҵ���
/// ͨ��ʹ��IInteractableEntity�̳д˽ӿ�
/// </summary>
public interface IInteractable
{
    Collider TriggerCollider { get;}
    GameObject GameObject { get; }
    
    /// <summary>
    /// ����ʱ��ʾ��Ϣ
    /// </summary>
    string InteractTipText { get; }
    /// <summary>
    /// ������ķ�Ӧ�ı�
    /// </summary>
    string InteractReactionText { get; }
    bool CanInteract { get; }
    public void OnInteract(GameObject player);
}