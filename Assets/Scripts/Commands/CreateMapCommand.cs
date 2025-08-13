using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMapCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var system = this.GetSystem<MapGenerationSystem>();
        system.ExecuteGenerationPipeline();
        //UIKit.OpenPanel<LoadingUI>().StartLoading("PlayScene");
    }
}
