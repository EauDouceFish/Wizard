using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

/// <summary>
/// ħ��ָ������¼����������ħ��
/// </summary>
[CreateAssetMenu(fileName = "MagicSpellGuide", menuName = "MagicSpellSystem/MagicSpellGuide")]
public class MagicSpellGuide : ScriptableObject
{
    public Dictionary<List<MagicElement>, SpecialSpellData> specialSpellGuideDict;

    [Header("��������ģ������")]
    [Tooltip("Spray���ͷ���ģ��")]
    public BasicSpellBaseData sprayTemplate;

    [Tooltip("Ball���ͷ���ģ��")]
    public BasicSpellBaseData ballTemplate;

    [Tooltip("Ray���ͷ���ģ��")]
    public BasicSpellBaseData rayTemplate;

    [Tooltip("Vine���ͷ���ģ��")]
    public BasicSpellBaseData vineTemplate;

    [Tooltip("Buff���ͷ���ģ��")]
    public BasicSpellBaseData buffTemplate;
}
