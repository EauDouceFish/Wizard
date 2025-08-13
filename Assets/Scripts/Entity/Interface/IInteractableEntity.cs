using UnityEngine;
using QFramework;

/// <summary>
/// �ɽ�����ʵ����࣬ͬʱ�̳�Entity��ʵ��IInteractable
/// </summary>
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class InteractableEntity : Entity, IInteractable
{
    [Header("��������")]
    [SerializeField] protected string interactTipText = "��E����";
    [SerializeField] protected string interactReactionText = "Ը��ӵ�����õ�һ��";
    [SerializeField] protected bool canInteract = true;

    [Header("��������Trigger")]
    [Tooltip("�����Ҫ��������ײCollider��TriggerCollider����ͬ�������£���Ҫ��Trigger˳����ǰ�������Ϸ�")]
    [SerializeField] protected Collider triggerCollider;

    public GameObject GameObject => gameObject;
    public string InteractTipText => interactTipText;
    public bool CanInteract => canInteract;
    Collider IInteractable.TriggerCollider => triggerCollider;

    public string InteractReactionText => interactReactionText;
    public bool HasTriggerCollider => triggerCollider != null;

    private InteractionSystem interactionSystem;

    protected virtual void Awake()
    {
        interactionSystem = this.GetSystem<InteractionSystem>();
    }

    protected virtual void Start()
    {
        interactionSystem.RegisterInteractable(this);
    }

    protected virtual void OnDestroy()
    {
        interactionSystem?.UnregisterInteractable(this);
    }

    /// <summary>
    /// �ж�����ҽ��뽻������
    /// </summary>
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerCollider != null)
        {
            this.SendEvent(new PlayerEnterInteractAreaEvent
            {
                Interactable = this,
                Player = other.gameObject
            });
        }
    }

    /// <summary>
    /// ����뿪�˽�������
    /// </summary>
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggerCollider != null)
        {
            this.SendEvent(new PlayerExitInteractAreaEvent
            {
                Interactable = this,
                Player = other.gameObject
            });
        }
    }

    public virtual void OnInteract(GameObject player)
    {
        Debug.Log($"{gameObject.name}: {interactReactionText}");
    }
}