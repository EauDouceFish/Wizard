using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Pursuit : Action
{
    public Enemy enemy;

    private SteeringBehaviours steeringBehaviours;
    private PursueBehaviour pursueBehaviour;
    private CollisionSensor collisionSensor;

    private Actor attackTarget;


    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
        steeringBehaviours = enemy.steeringBehaviours;
        pursueBehaviour = enemy.pursueBehaviour;
        collisionSensor = enemy.collisionSensor;
        attackTarget = enemy.AttackTarget;
    }

    public override TaskStatus OnUpdate()
    {
        if (attackTarget == null)
        {
            return TaskStatus.Failure;
        }

        Rigidbody targetRigidbody = attackTarget.GetComponent<Rigidbody>();

        Vector3 steering = pursueBehaviour.GetSteering(targetRigidbody);

        // 检测碰撞传感器，调整加速度方向
        if (collisionSensor != null)
        {
            Vector3 accDir = steering.normalized;
            if (collisionSensor.GetCollisionFreeDirection(accDir, out accDir))
            {
                accDir *= steering.magnitude;
                steering = accDir;
            }
            //Debug.Log($"CollisionFreeDirection: {accDir}");
        }
        else
        {
                        Debug.LogWarning("CollisionSensor is not assigned.");
        
        }

        steeringBehaviours.Steer(steering);
        steeringBehaviours.LookMoveDirection();
        //Debug.Log("Pursye");
        return TaskStatus.Success;
    }
}