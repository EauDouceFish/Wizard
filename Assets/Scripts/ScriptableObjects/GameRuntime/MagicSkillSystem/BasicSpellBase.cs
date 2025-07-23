using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

[CreateAssetMenu(fileName = "BasicSpellTest", menuName = "MagicSpellSystem/BasicSpellTest")]
[Serializable]
public class BasicSpellBase : SpellBase
{
    [Tooltip("基础技能释放的形式，后续由具体组合来定")]
    [SerializeField] BasicSpellType basicSpellType;
}
