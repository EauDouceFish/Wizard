using UnityEngine;
using QFramework;

/// <summary>
/// 可交互的实体基类，同时继承Entity并实现IInteractable
/// </summary>
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class InteractableEntity : Entity, IInteractable
{
    [Header("交互设置")]
    [SerializeField] protected string interactTipText = "按E交互";
    [SerializeField] protected string interactReactionText = "愿你拥有美好的一天";
    [SerializeField] protected bool canInteract = true;

    [Header("交互区域Trigger")]
    [Tooltip("如果需要把物理碰撞Collider和TriggerCollider放在同个物体下，需要将Trigger顺序提前，放在上方")]
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
    /// 判断是玩家进入交互区域
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
    /// 玩家离开了交互区域
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