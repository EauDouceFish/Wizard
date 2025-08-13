using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyEntityData", menuName = "EntitySystem/EnemyEntityData")]
public class EnemyEntityData : ScriptableObject
{
    [Header("����ʵ������")]
    [Tooltip("��������")]
    public string enemyName = "Default Enemy";

    [Tooltip("�������Ѫ��")]
    public int enemyBaseHealth = 50;

    [Tooltip("�������������")]
    public int enemyBaseAttack = 10;

    [Tooltip("������Ұ��Χ")]
    public float enemyVisionRange = 25f;

    [Tooltip("���﹥������")]
    public float enemyAttackRange = 10f;

    [Tooltip("����Ĭ�Ϲ�������Layer")]
    public LayerMask enemyAttackLayerMask;

    [Header("����Ʒ��")]
    [Tooltip("����Ʒ�ʣ�1-4��1Ϊ���Ʒ�ʣ�4Ϊ���Ʒ�ʡ���չ���Ǹ�ΪTag��ö��ֵ��")]
    public int enemyQuality = 1;

    [Header("�Ӿ�Ч��")]
    [Tooltip("����������Ч")]
    public GameObject deathVFX;
}
