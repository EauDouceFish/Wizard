using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class SpeedUpPermanent : EntityBuffBase
{
    private IHasBuffSpeedMultiplier actor;
    public override void AfterBeAdded()
    {
        base.AfterBeAdded();
        actor = Owner as IHasBuffSpeedMultiplier;
        if (actor != null)
        {
            actor.BuffSpeedMultiplier += 0.2f;
        }
    }
}
