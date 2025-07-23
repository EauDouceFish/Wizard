using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class Biome
{
    /// <summary>
    /// ȺϵID�����������;���
    /// </summary>
    public int Id => (int)BiomeType;                   
    public string BiomeName
    {
        get
        {
            return BiomeType.ToString();
        }
    }
    
    // SO�ֶ�����
    public BiomeType BiomeType { get; set; }           // ��ǰȺϵ����
    public Texture2D BiomeClassicTex { get; set; }     // Ⱥϵ��������
    public Color Color { get; set; }                   // Ⱥϵ��ɫ    

    public Material CommonMaterial { get; set; }        // Ⱥϵͨ�ò���

    public Biome(BiomeSO biomeSO)
    {
        BiomeType = biomeSO.biomeType;
        BiomeClassicTex = biomeSO.classicTexture;
        CommonMaterial = biomeSO.commonMaterial;
        Color = biomeSO.color;
    }

}
