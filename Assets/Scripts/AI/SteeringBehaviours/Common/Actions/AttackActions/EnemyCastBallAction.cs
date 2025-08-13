using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class EnemyCastBallAction : Action
{
    private Enemy enemy;
    private MagicSpellSystem magicSpellSystem;
    private float elapsedTime = 0f;

    [Header("ʩ������")]
    public float castInterval = 4f; // ʩ���������Vine�Գ���������Ҫ����׼��ʱ�䣩
    public float castPreparationTime = 1f; // ǰҡʱ�䣨��Vine��������Ҫ��׼��
    public float maxRotationSpeed = 120f; // �����ת���ٶ� degree/s

    [Header("�������")]
    public float launchHeight = 2f; // ����߶�ƫ��

    private Vector3 targetDirection;
    private Quaternion targetRotation;

    public override void OnStart()
    {
        enemy = GetComponent<Enemy>();
        magicSpellSystem = enemy.GetSystem<MagicSpellSystem>();
        elapsedTime = 0f;

        // ��ʼ����תĿ��Ϊ��ǰ����
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
            // û����ǰҡǰ��������׼Ŀ��
            HandleAiming();
        }
        else if (elapsedTime < castInterval)
        {
            // ǰҡ�׶Σ�������׼���ٶȷ���
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
    /// ��׼�׶� - �����ٶ�׷��Ŀ��
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