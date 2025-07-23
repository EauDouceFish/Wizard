using UnityEngine;

/// <summary>
/// Ҫ����������Ҫʵ�ִ˽ӿڣ�ʵ��ColliderTrigger����߼����Ҵ���
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
    void OnInteract(GameObject player);
}