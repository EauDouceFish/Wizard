using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

[CreateAssetMenu(fileName = "BasicSpellTest", menuName = "MagicSpellSystem/BasicSpellTest")]
[Serializable]
public class BasicSpellBaseData : SpellBaseData
{
    /// <summary>
    /// 技能释放的形式，球、射线、喷射、藤蔓、增益
    /// </summary>
    [Tooltip("基础技能释放的形式，后续由具体组合来定")]
    public BasicSpellType basicSpellType;

    [Header("伤害属性")]
    [Tooltip("基础伤害值")]
    public float baseDamage = 10f;

    [Tooltip("伤害触发间隔")]
    public float damageInterval = 1f;

    [Tooltip("伤害范围半径（Ball）")]
    public float damageRadius = 2f;

    [Header("施法属性")]
    [Tooltip("法术长度")]
    public float castRange = 50f;


    [Tooltip("法术最大宽度（Vine、Spray）")]
    public float spellWidth = 2f;

    [Tooltip("默认伤害高度")]
    public float spellHeight = 5f;

    [Header("视觉效果")]
    [Tooltip("技能预制体")]
    public GameObject spellPrefab;

    [Tooltip("特效颜色")]
    public Color spellColor = Color.white;

    [Tooltip("施法位置")]
    public bool isCastFromHand = true;
}
