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
    /// ������ҽ����¼�����E����
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
    /// ��ҽ���Ŀ�꽻��������ӵ���Χ���б����µ�ǰĿ��
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
    /// ����뿪Ŀ�꽻�����򣬴ӷ�Χ���б��Ƴ������µ�ǰĿ�ꡣ
    /// Ϊ�˱�֤�ָУ��˴������߼��ǣ����������뿪ʱ����������Χ���������ɽ������壬���Զ��л��������һ����
    /// �����Ͳ���Ҫÿ�ζ��ȴӽ��������뿪���л�Ŀ��
    /// </summary>
    private void OnPlayerExitInteractArea(PlayerExitInteractAreaEvent evt)
    {
        allInteractableObjectsInRange.Remove(evt.Interactable);
        UpdateCurrentTarget();
    }

    // ���µ�ǰ����Ŀ��Ϊ����Ŀɽ�������
    private void UpdateCurrentTarget()
    {
        IInteractable newTarget = GetClosestInteractable();
        SetCurrentTarget(newTarget);
    }

    // ��ȡ����Ŀɽ�������
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
        // ������һ�����������
        if (currentInteractingObject != null)
        {
            this.GetSystem<OutlineSystem>().HideOutline(currentInteractingObject.GameObject);
        }

        currentInteractingObject = target;

        // ��ʾ�µ�����
        if (currentInteractingObject != null && currentInteractingObject.CanInteract)
        {
            this.GetSystem<OutlineSystem>().ShowOutline(currentInteractingObject.GameObject);
        }
    }
}