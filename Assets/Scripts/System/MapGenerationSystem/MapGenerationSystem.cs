using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class MapGenerationSystem : AbstractSystem, IMapGenerationSystem
{
    GameCoreModel gameModel;
    MapModel mapModel;
    //BiomeModel biomeModel;

    // 可定制的地图生成管线
    List<IMapGenerationStep> pipeline = new();

    protected override void OnInit()
    {
        //biomeModel = this.GetModel<BiomeModel>();
        gameModel = this.GetModel<GameCoreModel>();
        mapModel = this.GetModel<MapModel>();

        // MapGeneration Pipeline
        pipeline.Add(new BaseMapFillTestStep());
        pipeline.Add(new DivideHexMapStep());
        pipeline.Add(new InitBiomeStep());
        pipeline.Add(new ConnectMainPathStep());
        pipeline.Add(new DiffuseCellsStep());
        pipeline.Add(new GroundModelFillStep());
        pipeline.Add(new BoundFillStep());
        pipeline.Add(new DecorationFillStep());
    }

    /// <summary>
    /// Generate the map base on the pipeline.
    /// </summary>
    /// <param name="mapConfigData"></param>
    public void ExecuteGenerationPipeline()
    {
        Debug.Log("Execute Map Generation Pipeline");
        foreach (IMapGenerationStep mapGenerationStep in pipeline)
        {
            mapGenerationStep.Execute(mapModel);
        }
        this.SendEvent<MapGenerationCompletedEvent>();
    }


    #region Architecture

    public MapModel GetMapModel()
    {
        if (mapModel != null)
        {
            return mapModel;
        }
        else
        {
            Debug.LogWarning("MapModel was not assigned!");
            return null;
        }
    }

    #endregion

}

// ��ͼ��С
public enum MapSize
{
    Default,
    Mini,
    Small,
    Medium,
    Large
}