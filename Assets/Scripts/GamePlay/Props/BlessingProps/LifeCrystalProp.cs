using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class LifeCrystalProp : AbstractBlessingProp
{
    public override void Execute()
    {
        base.Execute();
        Player.MaxHealth += 25;
        Player.CurrentHealth += 25;
    }
}