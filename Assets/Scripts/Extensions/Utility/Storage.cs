using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public partial class Storage : IUtility
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
    /// 获取默认的装饰物分布配置，如后续有特殊地图需求可额外配置。
    /// </summary>
    /// <returns></returns>
    public DecorationDistributionConfig GetDecorationDistributionConfig()
    {
        DecorationDistributionConfig decorationDistributionConfig = Resources.Load<DecorationDistributionConfig>("Config/MapGenerationSystem/DefaultDecorationDistributionConfig");
        return decorationDistributionConfig;
    }

    /// <summary>
    /// 获取一级装饰物
    /// </summary>
    public GameObject[] GetLevel1DecorationModels(Biome biome)
    {
        GameObject[] level1Models = Resources.LoadAll<GameObject>($"Prefabs/Decoration_01/{biome.BiomeName}");
        return level1Models;
    }

    /// <summary>
    /// 获取二级装饰物
    /// </summary>
    public GameObject[] GetLevel2DecorationModels(Biome biome)
    {
        GameObject[] level2Models = Resources.LoadAll<GameObject>($"Prefabs/Decoration_02/{biome.BiomeName}");
        return level2Models;
    }

    public Material GetOutlineMaterial()
    {
        Material material = Resources.Load<Material>("Materials/VFX/OutlineMaterial");
        return material;
    }

    public GameObject GetPlayerPrefab()
    {
        GameObject playerObject = Resources.Load<GameObject>("Prefabs/Character/Player/Player");
        return playerObject;
    }

    public MagicSpellGuide GetMagicSpellGuide()
    {
        MagicSpellGuide magicSpellGuide = Resources.Load<MagicSpellGuide>("Config/MagicSpellSystem/MagicSpellGuide");
        return magicSpellGuide;
    }

    public GameObject GetHealthUIPrefab()
    {
        GameObject healthUIPrefab = Resources.Load<GameObject>("Prefabs/UI/HealthUI");
        return healthUIPrefab;  
    }

    public GameObject GetStatusUIPrefab()
    {
        GameObject statusUIPrefab = Resources.Load<GameObject>("Prefabs/UI/StatusUI");
        return statusUIPrefab;
    }

    public List<ColorMap> GetColorMaps()
    {
        var colorMaps = Resources.Load<ColorMapSO>("Config/MagicSpellSystem/ColorMapSO");
        return colorMaps.colorMap;
    }

    public GameObject GetElementPillarPrefab()
    {
        GameObject elementPillarPrefab = Resources.Load<GameObject>("Prefabs/Gameplay/Props/ElementPillar");
        return elementPillarPrefab;
    }

    public GameObject GetEntityGroupPrefab()
    {
        GameObject entityGroupPrefab = Resources.Load<GameObject>("Prefabs/Gameplay/Entity/EntityGroup");
        return entityGroupPrefab;
    }

    public BattleDifficultyConfig GetBattleDifficultyConfig()
    {
        return Resources.Load<BattleDifficultyConfig>("Config/Battle/DefaultBattleDifficultyConfig");
    }

    public GameObject[] GetAllEnemyPrefabs()
    {
        return Resources.LoadAll<GameObject>("Prefabs/Gameplay/Entity/Enemy");
    }

    public GameObject GetHolyBlessingWaterPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Gameplay/Props/HolyBlessingWater");
    }

    public GameObject GetBossEnemyPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Gameplay/Entity/Enemy/BossEnemy");
    }

    public GameObject[] GetAllBlessingProps()
    {
        return Resources.LoadAll<GameObject>("Prefabs/Gameplay/Props/BlessingProps");
    }

    public Sprite GetElementIcon(MagicElement magicElement)
    {
        return Resources.Load<Sprite>($"Icon/{magicElement}Icon");
    }

    public Sprite GetBossIcon()
    {
         return Resources.Load<Sprite>("Icon/BossIcon");
    }

    public Sprite GetPlayerIcon()
    {
        return Resources.Load<Sprite>("Icon/PlayerIcon");
    }

    public GameObject GetGameEnderPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Gameplay/Props/GameEnder");
    }

    public MapConfigData GetDefaultMapConfigData()
    {
        return Resources.Load<MapConfigData>("Config/MapGenerationSystem/DefaultMapConfigData");
    }
}
