using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

public class NatureElementMasterProp : AbstractBlessingProp
{
    public float parameter = 0.18f;
    public override void Execute()
    {
        base.Execute();
        MagicSpellModel magicSpellModel = this.GetModel<MagicSpellModel>();
        magicSpellModel.RockElementMultiplier += parameter;
    }
}
