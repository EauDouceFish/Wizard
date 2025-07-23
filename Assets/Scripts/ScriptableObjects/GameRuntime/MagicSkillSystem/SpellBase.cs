using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

[Serializable]
public class SpellBase : ScriptableObject
{
    [Tooltip("基础咒语、特殊咒语")]
    [SerializeField] public SpellType spellType;

    [Tooltip("施法方式为瞬发/蓄力/吟唱")]
    [SerializeField] public CastType castType;
}
