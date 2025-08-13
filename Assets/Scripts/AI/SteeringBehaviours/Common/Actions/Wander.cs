using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Wander : Action
{
    private WanderBehaviour wanderBehaviour;
    private SteeringBehaviours steeringBehaviours;
    private CollisionSensor collisionSensor;

    public override void OnStart()
    {
        wanderBehaviour = GetComponent<WanderBehaviour>();
        steeringBehaviours = GetComponent<SteeringBehaviours>();
        collisionSensor = GetComponent<CollisionSensor>();
    }

    public override TaskStatus OnUpdate()
    {
        if (wanderBehaviour == null)
        {
            Debug.LogWarning("还没有实现Wander行为!");
            return TaskStatus.Failure;
        }

        // 获取漫游行为的加速度，然后应用加速度到角色身上
        Vector3 steering = wanderBehaviour.GetSteering();

        // 检测碰撞传感器，调整加速度方向
        if (collisionSensor != null)
        {
            Vector3 accDir = steering.normalized;
            if (collisionSensor.GetCollisionFreeDirection(accDir, out accDir))
            {
                accDir *= steering.magnitude;
                steering = accDir;
            }
        }

        if (steeringBehaviours != null)
        {
            steeringBehaviours.Steer(steering);
            steeringBehaviours.LookMoveDirection();
        }

        return TaskStatus.Success;
    }
}