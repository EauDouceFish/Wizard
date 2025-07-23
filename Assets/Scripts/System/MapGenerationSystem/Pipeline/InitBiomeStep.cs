using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// ��ʼ��Ⱥϵ��ʼλ��
/// </summary>
public class InitBiomeStep : IMapGenerationStep
{
    public void Execute(MapModel mapModel)
    {
        HexCell[] cells = mapModel.HexGrid.GetRandomCells(mapModel.BiomeConfigData.Count);

        for (int i = 0; i < mapModel.BiomeConfigData.Count; i++)
        {
            BiomeSO biomeData = mapModel.BiomeConfigData[i];
            Biome biome = new(biomeData);
            HexCell initCell = cells[i];

            // ��ʼ���Ӹ�����
            MeshRenderer renderer = initCell.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.SetFloat("_Metallic", 1.0f);
            }
            Debug.Log($"Initializing Biome: {biome.BiomeName} at position ({initCell.coord.x}, {initCell.coord.y})");

            mapModel.HexGrid.CreateHexRealm(initCell, biome);
        }
    }
}
