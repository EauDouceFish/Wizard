using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class Biome
{
    /// <summary>
    /// 群系ID，由索引类型决定
    /// </summary>
    public int Id => (int)BiomeType;                   
    public string BiomeName
    {
        get
        {
            return BiomeType.ToString();
        }
    }
    
    // SO字段数据
    public BiomeType BiomeType { get; set; }           // 当前群系类型
    public Texture2D BiomeClassicTex { get; set; }     // 群系经典纹理
    public Color Color { get; set; }                   // 群系颜色    

    public Material CommonMaterial { get; set; }        // 群系通用材质

    public Biome(BiomeSO biomeSO)
    {
        BiomeType = biomeSO.biomeType;
        BiomeClassicTex = biomeSO.classicTexture;
        CommonMaterial = biomeSO.commonMaterial;
        Color = biomeSO.color;
    }

}
