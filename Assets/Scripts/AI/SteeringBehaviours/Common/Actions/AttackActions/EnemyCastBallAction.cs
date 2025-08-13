using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class EnemyCastBallAction : Action
{
    private Enemy enemy;
    private MagicSpellSystem magicSpellSystem;
    private float elapsedTime = 0f;

    [Header("施法参数")]
    public float castInterval = 4f; // 施法间隔（比Vine稍长，火球需要更多准备时间）
    public float castPreparationTime = 1f; // 前摇时间（比Vine更长，需要瞄准）
    public float maxRotationSpeed = 120f; // 最大旋转角速度 degree/s

    [Header("抛射参数")]
    public float launchHeight = 2f; // 发射高度偏移

    private Vector3 targetDirection;
    private Quaternion targetRotation;

    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
        magicSpellSystem = enemy.GetSystem<MagicSpellSystem>();
        elapsedTime = 0f;

        // 初始化旋转目标为当前朝向
        targetDirection = enemy.transform.forward;
        targetRotation = Quaternion.LookRotation(targetDirection);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy == null || !enemy.IsInAttackRange || enemy.AttackTarget == null)
            return TaskStatus.Failure;

        elapsedTime += Time.deltaTime;

        if (elapsedTime <= castInterval - castPreparationTime)
        {
            // 没进入前摇前，持续瞄准目标
            HandleAiming();
        }
        else if (elapsedTime < castInterval)
        {
            // 前摇阶段，继续瞄准但速度放慢
            HandlePreparation();
        }

        if (elapsedTime >= castInterval)
        {
            CastBallSpell();
            elapsedTime = 0f;
        }

        return TaskStatus.Running;
    }

    /// <summary>
    /// 瞄准阶段 - 正常速度追踪目标
    /// </summary>
    private void HandleAiming()
    {
        UpdateTargetDirection();

        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            maxRotationSpeed * Time.deltaTime
        );
    }

    private void HandlePreparation()
    {
        UpdateTargetDirection();

        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            (maxRotationSpeed * 0.5f) * Time.deltaTime
        );
    }

    private void UpdateTargetDirection()
    {
        if (!enemy.AttackTarget) return;

        GameObject targetGameObject = enemy.AttackTarget.gameObject;
        Vector3 currentTargetPos = targetGameObject.transform.position;
        targetDirection = (currentTargetPos - enemy.transform.position).normalized;
        targetRotation = Quaternion.LookRotation(targetDirection);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
    }

    private void CastBallSpell()
    {
        BasicSpellInstance spellInstance = magicSpellSystem.CreateBasicSpellInstanceByEnum(BasicSpellType.Ball);

        Vector3 targetPosition = enemy.AttackTarget.transform.position;
        spellInstance.Initialize(enemy, targetPosition, false, null, 2f);
        spellInstance.Execute();
    }
}