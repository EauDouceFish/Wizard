using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class HealthPotionProp : AbstractBlessingProp
{
    public override void Execute()
    {
        base.Execute();
        Player.CurrentHealth += 100;
    }
}
