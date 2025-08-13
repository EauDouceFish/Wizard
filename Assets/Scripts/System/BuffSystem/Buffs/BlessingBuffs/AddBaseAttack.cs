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

    // 其实是永久逻辑，可以不考虑移除
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
            Debug.Log($"攻击力增加: + {bonus}, 当前等级: {CurrentLevel}");
        }
        else
        {
            Debug.Log("目标不能攻击，添加攻击Buff无意义");
        }
    }
}