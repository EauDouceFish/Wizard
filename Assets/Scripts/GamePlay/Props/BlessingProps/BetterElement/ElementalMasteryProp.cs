using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using PlayerSystem;

public class ElementalMasteryProp : AbstractBlessingProp
{
    public float parameter = 0.05f;
    public override void Execute()
    {
         base.Execute();
         MagicSpellModel magicSpellModel = this.GetModel<MagicSpellModel>();
         magicSpellModel.FireElementMultiplier += parameter;
         magicSpellModel.WaterElementMultiplier += parameter;
         magicSpellModel.IceElementMultiplier += parameter;
         magicSpellModel.NatureElementMultiplier += parameter;
         magicSpellModel.RockElementMultiplier += parameter;
    }
}
