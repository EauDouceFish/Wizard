using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using QFramework;
using Unity.VisualScripting;

public class AIActor : Actor
{
    /// <summary>
    /// ����AI����Ұ��Χ������˷�Χ��AI�᳢��Ѱ��Ŀ��
    /// </summary>
    [Range(0.1f, 100)]
    public float viewRadius = 10;

    /// <summary>
    /// �������룬����˾����AI�᳢�Թ���
    /// </summary>
    [Range(0.1f, 100)]
    public float attackRadius = 1;

    protected BehaviorTree behaviorTree;

    [HideInInspector] public SteeringBehaviours steeringBehaviours;
    [HideInInspector] public WanderBehaviour wanderBehaviour;
    [HideInInspector] public PursueBehaviour pursueBehaviour;
    [HideInInspector] public CollisionSensor collisionSensor;

    protected virtual void Awake()
    {
        steeringBehaviours = this.GetOrAddComponent<SteeringBehaviours>();
        wanderBehaviour = this.GetOrAddComponent<WanderBehaviour>();
        pursueBehaviour = this.GetOrAddComponent<PursueBehaviour>();
        collisionSensor = this.GetOrAddComponent<CollisionSensor>();

        // ������ת����Ϊ������Ϊ��
        behaviorTree = GetComponent<BehaviorTree>();
        if (behaviorTree != null)
        {
            behaviorTree.EnableBehavior();
        }
        else
        {
            Debug.LogWarning($"{name}: BehaviorTree component is missing!");
        }
    }

    protected virtual void Start()
    {

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// ��ȡ��ǰAI��Χ��Actor
    /// </summary>
    public List<Actor> GetActorsInView()
    {
        if (actorManager != null)
        {
            return actorManager.GetActorsWithinViewRange(this, transform.position, viewRadius);
        }
        return new List<Actor>();
    }

    /// <summary>
    /// ����Actor��Ұ�ڵ�Actor
    /// </summary>
    public List<Actor> GetActorsInView(ActorTypeFilter actorTypeFilter)
    {
        if (actorManager != null)
        {
            return actorManager.GetActorsWithinViewRange(this, transform.position, viewRadius, actorTypeFilter);
        }
        return new List<Actor>();
    }
}