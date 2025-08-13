using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyEntityData", menuName = "EntitySystem/EnemyEntityData")]
public class EnemyEntityData : ScriptableObject
{
    [Header("基础实体属性")]
    [Tooltip("怪物名称")]
    public string enemyName = "Default Enemy";

    [Tooltip("怪物基础血量")]
    public int enemyBaseHealth = 50;

    [Tooltip("怪物基础攻击力")]
    public int enemyBaseAttack = 10;

    [Tooltip("怪物视野范围")]
    public float enemyVisionRange = 25f;

    [Tooltip("怪物攻击距离")]
    public float enemyAttackRange = 10f;

    [Tooltip("怪物默认攻击对象Layer")]
    public LayerMask enemyAttackLayerMask;

    [Header("怪物品质")]
    [Tooltip("怪物品质，1-4，1为最低品质，4为最高品质。拓展考虑改为Tag、枚举值等")]
    public int enemyQuality = 1;

    [Header("视觉效果")]
    [Tooltip("死亡粒子特效")]
    public GameObject deathVFX;
}
