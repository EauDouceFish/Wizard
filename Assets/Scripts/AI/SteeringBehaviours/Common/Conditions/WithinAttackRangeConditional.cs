using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

/// <summary>
/// Enemyͨ�������жϣ��Ƿ��ڹ�����Χ��
/// </summary>
public class WithinAttackRangeConditional : Conditional
{
    private Enemy enemy;

    public override void OnAwake()
    {
        enemy = GetComponent<Enemy>();
    }


    public override TaskStatus OnUpdate()
    {
        if (enemy != null && enemy.AttackTarget != null && IsInAttackRange(enemy.AttackTarget))
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

    private bool IsInAttackRange(Actor actor)
    {
        if (actor == null) return false;
        return Vector3.Distance(actor.transform.position, transform.position) < enemy.attackRadius;
    }
}