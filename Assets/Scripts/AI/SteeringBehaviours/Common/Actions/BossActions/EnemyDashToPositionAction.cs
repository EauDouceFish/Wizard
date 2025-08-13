using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyDashToPositionAction : EnemyAction
{
    private SteeringBehaviours steeringBehaviours;

    public float maxDashTime = 2f;
    public float dashAccel = 20f;

    private Vector3 destination;
    private float dashTimer = 0f;
    private bool targetLocked = false;

    public override void OnStart()
    {
        base.OnStart();
        steeringBehaviours = enemy.steeringBehaviours;
        dashTimer = 0f;
        targetLocked = false;
        SetAnimationBool(true);
    }

    public override TaskStatus OnUpdate()
    {
        if (targetLocked == false)
        {
            targetLocked = true;
            destination = enemy.AttackTarget.transform.position;
            return TaskStatus.Running;
        }

        dashTimer += Time.deltaTime;

        Vector3 steering = steeringBehaviours.Seek(destination, dashAccel);
        steeringBehaviours.Steer(steering);
        steeringBehaviours.LookMoveDirection();

        if (dashTimer >= maxDashTime)
        {
            SetAnimationBool(false);
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}