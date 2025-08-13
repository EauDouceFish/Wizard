using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;
using QFramework;

public class EnemyCastSprayAction : Action
{
    private Enemy enemy;
    private MagicSpellSystem magicSpellSystem;
    private float elapsedTime = 0f;

    [Header("ʩ������")]
    public float aimingTime = 2f; // ��׼ǰҡʱ�䣨��Ray��һЩ��
    public float castingTime = 3f; // ���ι�������ʱ��
    public float maxRotationSpeed = 90f; // �����ת���ٶ� degree/s

    private BasicSpellInstance spraySpellInstance;
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

        // ��ʼ����תĿ��Ϊ��ǰ���򣬱���˲���׼
        targetDirection = enemy.transform.forward;
        targetRotation = Quaternion.LookRotation(targetDirection);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
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
        spraySpellInstance = magicSpellSystem.CreateBasicSpellInstanceByEnum(BasicSpellType.Spray);
        spraySpellInstance.Initialize(enemy, enemy.AttackTarget.transform.position, true);
        spraySpellInstance.Execute();
        isAiming = false;
        isCasting = true;
    }

    private void HandleCastingPhase()
    {
        // Spray�����ڼ����׷��Ŀ�꣬���ٶ���Խ���
        UpdateTargetDirection();

        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            maxRotationSpeed * Time.deltaTime
        );

        UpdateSprayDirection();
    }

    private void HandleAimingPhase()
    {
        UpdateTargetDirection();

        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            targetRotation,
            maxRotationSpeed * Time.deltaTime
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

    /// <summary>
    /// SprayEnemy�ᳯ����ǰ���������ι���
    /// </summary>
    private void UpdateSprayDirection()
    {
        if (spraySpellInstance != null && enemy.AttackTarget != null)
        {
            spraySpellInstance.transform.position = enemy.transform.position;
            spraySpellInstance.UpdateChannelTarget(enemy.transform.forward);
        }
    }

    private void StopCasting()
    {
        if (spraySpellInstance != null)
        {
            spraySpellInstance.DestroySelf();
            spraySpellInstance = null;
        }
        isCasting = false;
    }

    public override void OnEnd()
    {
        StopCasting();
    }
}