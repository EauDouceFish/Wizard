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
    /// �����ͷŵ���ʽ�������ߡ����䡢����������
    /// </summary>
    [Tooltip("���������ͷŵ���ʽ�������ɾ����������")]
    public BasicSpellType basicSpellType;

    [Header("�˺�����")]
    [Tooltip("�����˺�ֵ")]
    public float baseDamage = 10f;

    [Tooltip("�˺��������")]
    public float damageInterval = 1f;

    [Tooltip("�˺���Χ�뾶��Ball��")]
    public float damageRadius = 2f;

    [Header("ʩ������")]
    [Tooltip("��������")]
    public float castRange = 50f;


    [Tooltip("��������ȣ�Vine��Spray��")]
    public float spellWidth = 2f;

    [Tooltip("Ĭ���˺��߶�")]
    public float spellHeight = 5f;

    [Header("�Ӿ�Ч��")]
    [Tooltip("����Ԥ����")]
    public GameObject spellPrefab;

    [Tooltip("��Ч��ɫ")]
    public Color spellColor = Color.white;

    [Tooltip("ʩ��λ��")]
    public bool isCastFromHand = true;
}
