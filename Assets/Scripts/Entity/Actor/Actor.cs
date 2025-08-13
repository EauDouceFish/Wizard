using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// ���壬��ҪЯ��Animator��Rigidbody�������Actor
/// ͳһ��ActorManageSystem����
/// </summary>
public class Actor : Entity, ICanAddElementStatus
{
    protected Animator animator;
    protected Rigidbody rb;
    protected ActorManageSystem actorManager;

    public HashSet<ElementStatusType> StatusTypes => statusTypes;

    private HashSet<ElementStatusType> statusTypes = new HashSet<ElementStatusType>();

    /// <summary>
    /// ��ʵ��
    /// </summary>
    protected virtual void Update()
    {

    }

    /// <summary>
    /// ��ʵ��
    /// </summary>
    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnEnable()
    {
        actorManager = this.GetSystem<ActorManageSystem>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            // ���ͬ��û���ҵ�����ݹ�����Ӷ���
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"{name}: No Animator found on this GameObject or its children!");
            }
        }
        rb = GetComponent<Rigidbody>();

        actorManager.RegisterActor(this);
    }

    protected virtual void OnDisable()
    {
        actorManager.UnregisterActor(this);
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    #region AddStatus
    void ICanAddElementStatus.AddStatus(ElementStatusType statusType)
    {
        statusTypes.Add(statusType);
    }

    void ICanAddElementStatus.RemoveStatus(ElementStatusType statusType)
    {
        statusTypes.Remove(statusType);
    }
    #endregion
}
