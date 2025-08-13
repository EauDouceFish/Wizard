using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class SpeedUpPermanentProp : AbstractBlessingProp
{
    public override void Execute()
    {
        base.Execute();
        Player.BuffSystem.AddBuff<SpeedUpPermanent>(this.name);
    }
}
