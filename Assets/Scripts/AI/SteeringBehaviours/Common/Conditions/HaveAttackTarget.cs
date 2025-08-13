using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasAttackTargetConditional : Conditional
{
    private Enemy enemy;

    public override void OnAwake()
    {
        enemy = GetComponent<Enemy>();
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy != null && enemy.HasAttackTarget)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}