using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Linq;

public class InteractionSystem : AbstractSystem, IInteractionSystem
{
    private List<IInteractable> interactableObjectsList = new();
    private List<IInteractable> allInteractableObjectsInRange = new();
    private IInteractable currentInteractingObject;
    private GameObject player;

    protected override void OnInit()
    {
        this.RegisterEvent<PlayerEnterInteractAreaEvent>(OnPlayerEnterInteractArea);
        this.RegisterEvent<PlayerExitInteractAreaEvent>(OnPlayerExitInteractArea);
        this.RegisterEvent<PlayerInteractEvent>(OnPlayerInteract);
    }

    public void RegisterInteractable(IInteractable interactable)
    {
        if (interactable != null && !interactableObjectsList.Contains(interactable))
        {
            interactableObjectsList.Add(interactable);
        }
    }

    public void UnregisterInteractable(IInteractable interactable)
    {
        if (interactable != null)
        {
            interactableObjectsList.Remove(interactable);
            allInteractableObjectsInRange.Remove(interactable);

            if (currentInteractingObject == interactable)
            {
                UpdateCurrentTarget();
            }
        }
    }

    /// <summary>
    /// 处理玩家交互事件（按E键）
    /// </summary>
    private void OnPlayerInteract(PlayerInteractEvent evt)
    {
        if (currentInteractingObject != null && currentInteractingObject.CanInteract)
        {
            currentInteractingObject.OnInteract(evt.Player);
        }
    }

    public IInteractable GetCurrentInteractingTarget()
    {
        return currentInteractingObject;
    }

    /// <summary>
    /// 玩家进入目标交互区域，添加到范围内列表并更新当前目标
    /// </summary>
    private void OnPlayerEnterInteractArea(PlayerEnterInteractAreaEvent evt)
    {
        player = evt.Player;

        if (!allInteractableObjectsInRange.Contains(evt.Interactable))
        {
            allInteractableObjectsInRange.Add(evt.Interactable);
        }

        UpdateCurrentTarget();
    }

    /// <summary>
    /// 玩家离开目标交互区域，从范围内列表移除并更新当前目标。
    /// 为了保证手感，此处更新逻辑是，当从区域离开时，如果玩家周围还有其他可交互物体，则自动切换到最近的一个。
    /// 这样就不需要每次都先从交互区域离开来切换目标
    /// </summary>
    private void OnPlayerExitInteractArea(PlayerExitInteractAreaEvent evt)
    {
        allInteractableObjectsInRange.Remove(evt.Interactable);
        UpdateCurrentTarget();
    }

    // 更新当前交互目标为最近的可交互物体
    private void UpdateCurrentTarget()
    {
        IInteractable newTarget = GetClosestInteractable();
        SetCurrentTarget(newTarget);
    }

    // 获取最近的可交互物体
    private IInteractable GetClosestInteractable()
    {
        if (player == null || allInteractableObjectsInRange.Count == 0)
        {
            return null;
        }

        List<IInteractable> objects = allInteractableObjectsInRange.Where(i => i.CanInteract).ToList();

        if (objects.Count == 0)
        {
            return null;
        }

        if (objects.Count == 1)
        {
            return objects[0];
        }

        IInteractable closest = null;
        float closestDistance = float.MaxValue;
        foreach (var interactable in objects)
        {
            float distance = Vector3.Distance(player.transform.position, interactable.GameObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        return closest;
    }

    private void SetCurrentTarget(IInteractable target)
    {
        // 隐藏上一个物体的轮廓
        if (currentInteractingObject != null)
        {
            this.GetSystem<OutlineSystem>().HideOutline(currentInteractingObject.GameObject);
        }

        currentInteractingObject = target;

        // 显示新的轮廓
        if (currentInteractingObject != null && currentInteractingObject.CanInteract)
        {
            this.GetSystem<OutlineSystem>().ShowOutline(currentInteractingObject.GameObject);
        }
    }
}