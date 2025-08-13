using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class EnemyCastVineAction : Action
{
    private Enemy enemy;
    private MagicSpellSystem magicSpellSystem;
    private float elapsedTime = 0f;
    public float waitTime = 0.9f; // 等待时间

    private Vector3 lockedDirection;
    private bool directionLocked = false;

    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
        magicSpellSystem = enemy.GetSystem<MagicSpellSystem>();
        elapsedTime = 0f;
        directionLocked = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy == null || enemy.AttackTarget == null)
            return TaskStatus.Failure;

        // 立刻锁定方向
        if (!directionLocked)
        {
            directionLocked = true;
            lockedDirection = (enemy.AttackTarget.transform.position - enemy.transform.position).normalized;

            // 面向锁定的方向
            Quaternion targetRotation = Quaternion.LookRotation(lockedDirection);
            enemy.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        elapsedTime += Time.deltaTime;

        // 等待0.5秒后发射Spray
        if (elapsedTime >= waitTime)
        {
            CastSpraySpell();
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void CastSpraySpell()
    {
        BasicSpellInstance spellInstance = magicSpellSystem.CreateBasicSpellInstanceByEnum(BasicSpellType.Vine);
        float yOffset = -GOExtensions.GetModelBoundsAABB(enemy.gameObject).size.y;
        spellInstance.Initialize(enemy, lockedDirection, true, null, yOffset);
        spellInstance.Execute();
    }
}