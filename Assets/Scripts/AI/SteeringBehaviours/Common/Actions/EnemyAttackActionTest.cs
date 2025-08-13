using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyAttackActionTest : Action
{
    private Enemy enemy;
    private float elapsedTime = 0.0f;

    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy.IsInAttackRange == false)
        {
            return TaskStatus.Failure;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 0.5f)
        {
            Debug.Log("attack");
            elapsedTime = 0.0f;
        }

        return TaskStatus.Running;
    }
}