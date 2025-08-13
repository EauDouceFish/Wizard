using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class SlowDown : EntityBuffBase
{
    private IHasBuffSpeedMultiplier actor;
    public override void AfterBeAdded()
    {
        Debug.Log("ÃÌº”¡ÀBuff£∫SlowDown");
        actor = Owner as IHasBuffSpeedMultiplier;
        if (actor != null)
        {
            actor.BuffSpeedMultiplier -= 0.5f;
        }
    }

    public override void AfterBeRemoved()
    {
        Debug.Log("“∆≥˝¡ÀBuff£∫SlowDown");
        if(actor != null)
        {
            actor.BuffSpeedMultiplier += 0.5f;
        }
    }
}
