using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// 魔法指引，记录了所有特殊魔法
/// </summary>
[CreateAssetMenu(fileName = "MagicSpellGuide", menuName = "MagicSpellSystem/MagicSpellGuide")]
public class MagicSpellGuide : ScriptableObject
{
    public Dictionary<List<MagicElement>, SpecialSpellData> specialSpellGuideDict;

    [Header("基础法术模板配置")]
    [Tooltip("Spray类型法术模板")]
    public BasicSpellBaseData sprayTemplate;

    [Tooltip("Ball类型法术模板")]
    public BasicSpellBaseData ballTemplate;

    [Tooltip("Ray类型法术模板")]
    public BasicSpellBaseData rayTemplate;

    [Tooltip("Vine类型法术模板")]
    public BasicSpellBaseData vineTemplate;

    [Tooltip("Buff类型法术模板")]
    public BasicSpellBaseData buffTemplate;
}
