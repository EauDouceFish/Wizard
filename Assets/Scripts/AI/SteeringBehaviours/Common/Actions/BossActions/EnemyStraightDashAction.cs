using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

/// <summary>
/// 横冲直撞，不管目标位置
/// </summary>
public class EnemyStraightDashAction : EnemyAction
{
    private SteeringBehaviours steeringBehaviours;

    public float maxDashTime = 1.2f;
    public float dashAccel = 22f;

    private Vector3 dashDirection;
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

    // 冲刺x秒以后退出
    public override TaskStatus OnUpdate()
    {
        if (targetLocked == false)
        {
            targetLocked = true;
            Vector3 targetPosition = enemy.AttackTarget.transform.position;
            dashDirection = (targetPosition - transform.position).normalized;
            dashDirection.y = 0;
            return TaskStatus.Running;
        }

        dashTimer += Time.deltaTime;

        Vector3 farTarget = transform.position + dashDirection * 1000f;
        Vector3 steering = steeringBehaviours.Seek(farTarget, dashAccel);
        steeringBehaviours.Steer(steering);
        steeringBehaviours.LookAtDirection(dashDirection);

        if (dashTimer >= maxDashTime)
        {
            SetAnimationBool(false);
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}