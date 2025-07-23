using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class Buff_AddAttack : EntityBuffBase
{
    public override void AfterBeAdded()
    {
        var attackable = Owner as IAttacker;
        if (attackable != null)
        {
            attackable.BaseAttack += 5f;
        }
        else
        {
            Debug.Log("Ŀ�겻�ܹ�������ӹ���Buff������");
        }
    }
}
