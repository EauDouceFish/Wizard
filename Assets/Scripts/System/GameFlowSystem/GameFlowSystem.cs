using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class GameFlowSystem : AbstractSystem, ICanRegisterEvent, ICanSendCommand
{
    protected override void OnInit()
    {
        this.RegisterEvent<MapGenerationCompletedEvent>(SummonPlayer);
    }

    private void SummonPlayer(MapGenerationCompletedEvent evt) 
    {
        this.SendCommand(new SummonPlayerCommand());
    }
}

