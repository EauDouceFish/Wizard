using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class Storage : IUtility
{
    public BiomeSO[] GetAllBiomeSO()
    {
        BiomeSO[] allBiomes = Resources.LoadAll<BiomeSO>("Config/Biomes");
        return allBiomes;
    }

    public GameObject[] GetAllGroundModels()
    {
        GameObject[] allGroundModels = Resources.LoadAll<GameObject>("Prefabs/GroundModels");
        return allGroundModels;
    }

    public GameObject[] GetAllEdgeObstacleModels()
    {
        GameObject[] obstacleModels = Resources.LoadAll<GameObject>("Prefabs/EdgeObstacleModels");
        return obstacleModels;
    }

    public EdgeFillConfig GetEdgeFillConfig()
    {
        EdgeFillConfig edgeFillConfig = Resources.Load<EdgeFillConfig>("Config/MapGenerationSystem/EdgeFillConfig");

        return edgeFillConfig;
    }

    /// <summary>
    /// ��ȡĬ�ϵ�װ����ֲ����ã�������������ͼ����ɶ������á�
    /// </summary>
    /// <returns></returns>
    public DecorationDistributionConfig GetDecorationDistributionConfig()
    {
        DecorationDistributionConfig decorationDistributionConfig = Resources.Load<DecorationDistributionConfig>("Config/MapGenerationSystem/DefaultDecorationDistributionConfig");
        return decorationDistributionConfig;
    }

    /// <summary>
    /// ��ȡһ��װ����
    /// </summary>
    public GameObject[] GetLevel1DecorationModels(Biome biome)
    {
        GameObject[] level1Models = Resources.LoadAll<GameObject>($"Prefabs/Decoration_01/{biome.BiomeName}");
        return level1Models;
    }

    /// <summary>
    /// ��ȡ����װ����
    /// </summary>
    public GameObject[] GetLevel2DecorationModels(Biome biome)
    {
        GameObject[] level2Models = Resources.LoadAll<GameObject>($"Prefabs/Decoration_02/{biome.BiomeName}");
        return level2Models;
    }

    public Material LoadOutlineMaterial()
    {
        Material material = Resources.Load<Material>("Materials/VFX/OutlineMaterial");
        return material;
    }

    public GameObject GetPlayerPrefab()
    {
        GameObject playerObject = Resources.Load<GameObject>("Prefabs/Character/Player/Player");
        return playerObject;
    }
}
