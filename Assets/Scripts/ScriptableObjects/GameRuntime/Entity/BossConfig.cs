using UnityEngine;

[CreateAssetMenu(fileName = "BossConfig", menuName = "EntitySystem/BossConfig")]
public class BossConfig : ScriptableObject
{
    [Header("攻击时机设置")]
    [Tooltip("攻击间隔时间")]
    public float attackInterval = 3f;

    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float pursuitSpeed = 9f;

    [Header("攻击持续时间")]
    public float vineSkillDuration = 5f;
    public float raySkillDuration = 3f;
    public float ballSkillDuration = 3.5f;
    public float spraySkillDuration = 4f;

    [Header("攻击频率")]
    [Tooltip("Ball攻击间隔")]
    public float ballCastInterval = 1f;
    [Tooltip("Vine攻击间隔")]
    public float vineCastInterval = 1.2f;

    [Header("攻击距离")]
    public float rayAttackRange = 40f;
    public float vineAttackRange = 10f;
    public float ballAttackRange = 35f;
    public float sprayAttackRange = 6f;
}