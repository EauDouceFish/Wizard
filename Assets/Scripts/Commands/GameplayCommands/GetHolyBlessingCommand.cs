using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public struct GetHolyBlessingEvent
{

}

/// <summary>
/// �������ʥף����������Դ�����Buff��������ѡһ�����
/// </summary>
public class GetHolyBlessingCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent(new GetHolyBlessingEvent());
    }
}
