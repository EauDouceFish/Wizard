using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Biome的静态配置数据，可动态扩充
/// </summary>
[CreateAssetMenu(fileName = "NewBiome", menuName = "MapGenerationSystem/BiomeSO")]
public class BiomeSO : ScriptableObject
{
    public Material commonMaterial;                   // 群系通用材质
    public BiomeType biomeType;                       // 当前群系类型
    public Texture2D classicTexture;                  // 群系经典纹理
    public Color color;                               // 群系代表颜色
}
