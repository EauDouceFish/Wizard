using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;
using QFramework;

public class EnemyCastRayAction : Action
{
    private Enemy enemy;
    private MagicSpellSystem magicSpellSystem;
    private float elapsedTime = 0f;

    private float aimingTime = 3f; // 瞄准前摇时间
    private float castingTime = 5f; // 射线持续时间
    private float maxRotationSpeed = 18f;

    private BasicSpellInstance raySpellInstance;
    private bool isAiming = true;
    private bool isCasting = false;
    private Vector3 targetDirection;
    private Quaternion targetRotation;

    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
        magicSpellSystem = enemy.GetSystem<MagicSpellSystem>();

        elapsedTime = 0f;
        isAiming = true;
        isCasting = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy == null || !enemy.IsInAttackRange || enemy.AttackTarget == null)
        {
            return TaskStatus.Failure;
        }

        elapsedTime += Time.deltaTime;

        if (isAiming)
        {
            HandleAimingPhase();

            if (elapsedTime >= aimingTime)
            {
                StartCasting();
            }
        }

        if (isCasting)
        {
            HandleCastingPhase();

            if (elapsedTime >= aimingTime + castingTime)
            {
                StopCasting();
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running;
    }

    private void StartCasting()
    {
        raySpellInstance = magicSpellSystem.CreateBasicSpellInstanceByEnum(BasicSpellType.Ray);
        raySpellInstance.Initialize(enemy, enemy.AttackTarget.transform.position, true, null, 0);
        raySpellInstance.Execute();
        isAiming = false;
        isCasting = true;
    }

    private void HandleCastingPhase()
    {
        // 限定最大旋转角速度
        //Debug.Log(maxRotationSpeed * Time.deltaTime);

        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            maxRotationSpeed * Time.deltaTime
        );

        UpdateTargetDirection();

        UpdateRayDirection();
    }

    private void HandleAimingPhase()
    {
        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            maxRotationSpeed * Time.deltaTime
        );
        UpdateTargetDirection();
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

    /// <summary>
    /// RayEnemy只会朝着正前方发射Ray
    /// </summary>
    private void UpdateRayDirection()
    {
        if (raySpellInstance != null && enemy.AttackTarget != null)
        {
            raySpellInstance.transform.position = enemy.transform.position;
            raySpellInstance.UpdateChannelTarget(enemy.transform.forward);
        }
    }

    private void StopCasting()
    {
        if (raySpellInstance != null)
        {
            raySpellInstance.DestroySelf();
            raySpellInstance = null;
        }
        isCasting = false;
    }

    public override void OnEnd()
    {
        StopCasting();
    }
}