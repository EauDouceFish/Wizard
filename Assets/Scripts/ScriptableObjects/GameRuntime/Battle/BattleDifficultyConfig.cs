using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultBattleDifficultyConfig",  menuName = "AIDirectorSystem/BattleDifficultyConfig")]
public class BattleDifficultyConfig : ScriptableObject
{
    [Header("敌人数量配置")]

    [Tooltip("在难度下，随机产生的敌人总数量")]
    public AnimationCurve enemyNumByDifficultyCurve;
    
    [Tooltip("在难度下，随机产生的敌人每一波次最大数量")]
    public AnimationCurve enemyMaxNumPerWaveByDifficultyCurve;

    [Header("敌人强度配置")]

    [Tooltip("在难度下，随机产生的敌人携带状态Buff的概率")]
    public AnimationCurve enemyCarryStatusRateByDifficultyCurve;

    [Tooltip("在难度下，会出现敌人品质的范围（1-4级）")]
    public List<Vector2Int> qualityConfigsByDifficulty;
}
