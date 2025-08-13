using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

[Serializable]
public class SpellBaseData : ScriptableObject
{
    [Tooltip("���������������")]
    [SerializeField] public SpellType spellType;

    /// <summary>
    /// ˲��/����/����
    /// </summary>
    [Tooltip("ʩ����ʽΪ˲��/����/����")]
    public CastType castType;
}
