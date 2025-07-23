using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

[Serializable]
public class SpellBase : ScriptableObject
{
    [Tooltip("���������������")]
    [SerializeField] public SpellType spellType;

    [Tooltip("ʩ����ʽΪ˲��/����/����")]
    [SerializeField] public CastType castType;
}
