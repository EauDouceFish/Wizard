using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMapCommand : AbstractCommand
{
    private MapSize size;

    public CreateMapCommand(MapSize size)
    {
        this.size = size;
    }

    protected override void OnExecute()
    {
        var model = this.GetModel<MapModel>();
        var system = this.GetSystem<MapGenerationSystem>();

        model.SetCurrentMapSize(size);
        system.ExecuteGenerationPipeline();
        //UIKit.OpenPanel<LoadingUI>().StartLoading("PlayScene");
    }
}
