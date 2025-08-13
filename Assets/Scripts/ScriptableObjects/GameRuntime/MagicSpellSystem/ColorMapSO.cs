using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorMap
{
    public MagicElement element;
    public Color color;
}

[CreateAssetMenu(fileName = "ColorMapSO", menuName = "MagicSpellSystem/ColorMap")]
public class ColorMapSO : ScriptableObject
{
    [Header("元素颜色设定")]
    public List<ColorMap> colorMap = new List<ColorMap>
    {
        new ColorMap { element = MagicElement.Fire, color = Color.red },
        new ColorMap { element = MagicElement.Water, color = Color.blue },
        new ColorMap { element = MagicElement.Ice, color = Color.cyan },
        new ColorMap { element = MagicElement.Nature, color = new Color(0.4f, 1f, 0.4f) },
        new ColorMap { element = MagicElement.Rock, color = new Color(0.5f, 0.3f, 0f) }
    };
}