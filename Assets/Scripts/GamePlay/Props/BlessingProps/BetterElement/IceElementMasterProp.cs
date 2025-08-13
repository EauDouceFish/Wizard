using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

public class IceElementMasterProp : AbstractBlessingProp
{
    public float parameter = 0.15f;
    public override void Execute()
    {
         base.Execute();
         MagicSpellModel magicSpellModel = this.GetModel<MagicSpellModel>();
         magicSpellModel.IceElementMultiplier += parameter;
    }
}
