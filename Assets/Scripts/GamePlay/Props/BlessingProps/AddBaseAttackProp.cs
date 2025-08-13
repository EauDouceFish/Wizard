using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

public class AddBaseAttackProp : AbstractBlessingProp
{
    public override void Execute()
    {
         base.Execute();
         Player.BuffSystem.AddBuff<AddBaseAttack>(this.name);
    }
}
