using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public struct GetHolyBlessingEvent
{

}

/// <summary>
/// 获得了神圣祝福的命令，可以从三个Buff道具中挑选一个获得
/// </summary>
public class GetHolyBlessingCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent(new GetHolyBlessingEvent());
    }
}
