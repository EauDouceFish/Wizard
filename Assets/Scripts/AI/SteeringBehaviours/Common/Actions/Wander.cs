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
            Debug.LogWarning("��û��ʵ��Wander��Ϊ!");
            return TaskStatus.Failure;
        }

        // ��ȡ������Ϊ�ļ��ٶȣ�Ȼ��Ӧ�ü��ٶȵ���ɫ����
        Vector3 steering = wanderBehaviour.GetSteering();

        // �����ײ���������������ٶȷ���
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