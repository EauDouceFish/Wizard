using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class MapGenerationSystem : AbstractSystem, IMapGenerationSystem
{
    MapModel mapModel;

    // 可定制的地图生成管线
    List<IMapGenerationStep> pipeline = new();

    protected override void OnInit()
    {
        mapModel = this.GetModel<MapModel>();

        // MapGeneration Pipeline
        pipeline.Add(new BaseMapFillTestStep());
        pipeline.Add(new DivideHexMapStep());
        pipeline.Add(new InitBiomeStep());

        //直接连接+随机传播
        pipeline.Add(new ConnectMainPathStep());  // --1
        pipeline.Add(new DiffuseCellsStep());     // --2
        
        //// 填充边界、装饰物
        pipeline.Add(new GroundModelFillStep());
        pipeline.Add(new BoundFillStep());
        pipeline.Add(new DecorationFillStep());

        // 填充地图内物品、怪物
        pipeline.Add(new RelocateBossStep());
        pipeline.Add(new FillPropStep());
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

public enum MapSize
{
    Default,
    Mini,
    Small,
    Medium,
    Large
}