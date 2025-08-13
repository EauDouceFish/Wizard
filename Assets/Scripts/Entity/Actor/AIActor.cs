using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using QFramework;
using Unity.VisualScripting;

public class AIActor : Actor
{
    /// <summary>
    /// 单个AI的视野范围，进入此范围后，AI会尝试寻找目标
    /// </summary>
    [Range(0.1f, 100)]
    public float viewRadius = 10;

    /// <summary>
    /// 攻击距离，进入此距离后，AI会尝试攻击
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

        // 最后根据转向行为构建行为树
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
    /// 获取当前AI周围的Actor
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
    /// 返回Actor视野内的Actor
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