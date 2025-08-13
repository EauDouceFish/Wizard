using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using BehaviorDesigner.Runtime.Tasks;
using System;

public class EnemyCastVine8DirAction : EnemyAction, IController
{
    public float actionDuration = 1.5f;     // ��������ʱ��
    public float animTriggerTime = 1f;    // �����¼�����ʱ��
    public bool skillCasted = false;        // �����Ƿ���ʩ��


    MagicSpellSystem magicSpellSystem;

    private float elapsedTime = 0f;


    public override void OnStart()
    {
        base.OnStart();
        magicSpellSystem = this.GetSystem<MagicSpellSystem>();
        // ���Ŷ���
        elapsedTime = 0f;
        skillCasted = false;
        SetAnimationTrigger();
    }


    public override TaskStatus OnUpdate()
    {
        elapsedTime += Time.deltaTime;
        if ((elapsedTime >= animTriggerTime) && skillCasted == false)
        {
            CastVine8DirSkill();
            skillCasted = true;
        }

        if (elapsedTime >= actionDuration)
        {
            SetAnimationBool(false);
            elapsedTime = 0f;
            skillCasted = false;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void CastVine8DirSkill()
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward,    // �� (0, 0, 1)
            Vector3.back,       // �� (0, 0, -1)
            Vector3.left,       // �� (-1, 0, 0)
            Vector3.right,      // �� (1, 0, 0)
            (Vector3.forward + Vector3.left).normalized,   // ����
            (Vector3.forward + Vector3.right).normalized,  // ����
            (Vector3.back + Vector3.left).normalized,      // ����
            (Vector3.back + Vector3.right).normalized      // ����
        };

        // ���㷢��λ�õ�Yƫ��
        float yOffset = -GOExtensions.GetModelBoundsAABB(enemy.gameObject).size.y + 0.3f;

        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 direction = directions[i];
            if (i == 0)
            {
                CastSingleVine(direction, yOffset);
            }
            else
            {
                // ���С�ӳ�
                StartCoroutine(CastVineWithDelay(direction, yOffset, i * 0.05f));
            }
        }
    }

    private void CastSingleVine(Vector3 direction, float yOffset)
    {
        Debug.Log($"Casting vine in direction: {direction}");
        BasicSpellInstance spellInstance = magicSpellSystem.CreateBasicSpellInstanceByEnum(BasicSpellType.Vine);
        Vector3 targetPosition = enemy.transform.position + direction * spellInstance.GetSpellData().castRange;
        spellInstance.Initialize(enemy, targetPosition, false, null, yOffset);
        spellInstance.Execute();
    }

    private IEnumerator CastVineWithDelay(Vector3 direction, float yOffset, float delay)
    {
        yield return new WaitForSeconds(delay);
        CastSingleVine(direction, yOffset);
    }


    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
