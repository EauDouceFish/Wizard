using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

[CreateAssetMenu(fileName = "BasicSpellTest", menuName = "MagicSpellSystem/BasicSpellTest")]
[Serializable]
public class BasicSpellBase : SpellBase
{
    [Tooltip("���������ͷŵ���ʽ�������ɾ����������")]
    [SerializeField] BasicSpellType basicSpellType;
}
