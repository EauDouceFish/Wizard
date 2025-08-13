using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultBattleDifficultyConfig",  menuName = "AIDirectorSystem/BattleDifficultyConfig")]
public class BattleDifficultyConfig : ScriptableObject
{
    [Header("������������")]

    [Tooltip("���Ѷ��£���������ĵ���������")]
    public AnimationCurve enemyNumByDifficultyCurve;
    
    [Tooltip("���Ѷ��£���������ĵ���ÿһ�����������")]
    public AnimationCurve enemyMaxNumPerWaveByDifficultyCurve;

    [Header("����ǿ������")]

    [Tooltip("���Ѷ��£���������ĵ���Я��״̬Buff�ĸ���")]
    public AnimationCurve enemyCarryStatusRateByDifficultyCurve;

    [Tooltip("���Ѷ��£�����ֵ���Ʒ�ʵķ�Χ��1-4����")]
    public List<Vector2Int> qualityConfigsByDifficulty;
}
