using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class AddBaseAttack : EntityBuffBase
{
    private float baseAttackBonus = 3f; 

    public override void AfterBeAdded()
    {
        
    }

    // ��ʵ�������߼������Բ������Ƴ�
    public override void AfterBeRemoved()
    {
        IAttacker attackable = Owner as IAttacker;
        if (attackable != null)
        {
            attackable.CurrentAttack -= baseAttackBonus * CurrentLevel;
        }
    }

    protected override void OnCurrentLevelChange(int change)
    {
        ApplyAttackBonus(change);
    }

    private void ApplyAttackBonus(int levelChange)
    {
        var attackable = Owner as IAttacker;
        if (attackable != null)
        {
            float bonus = baseAttackBonus * levelChange;
            attackable.CurrentAttack += bonus;
            Debug.Log($"����������: + {bonus}, ��ǰ�ȼ�: {CurrentLevel}");
        }
        else
        {
            Debug.Log("Ŀ�겻�ܹ�������ӹ���Buff������");
        }
    }
}