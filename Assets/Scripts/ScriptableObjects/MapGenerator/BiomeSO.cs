using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Biome�ľ�̬�������ݣ��ɶ�̬����
/// </summary>
[CreateAssetMenu(fileName = "NewBiome", menuName = "MapGenerationSystem/BiomeSO")]
public class BiomeSO : ScriptableObject
{
    public Material commonMaterial;                   // Ⱥϵͨ�ò���
    public BiomeType biomeType;                       // ��ǰȺϵ����
    public Texture2D classicTexture;                  // Ⱥϵ��������
    public Color color;                               // Ⱥϵ������ɫ
}
