using UnityEngine;

[CreateAssetMenu(fileName = "BossConfig", menuName = "EntitySystem/BossConfig")]
public class BossConfig : ScriptableObject
{
    [Header("����ʱ������")]
    [Tooltip("�������ʱ��")]
    public float attackInterval = 3f;

    [Header("�ƶ�����")]
    public float moveSpeed = 5f;
    public float pursuitSpeed = 9f;

    [Header("��������ʱ��")]
    public float vineSkillDuration = 5f;
    public float raySkillDuration = 3f;
    public float ballSkillDuration = 3.5f;
    public float spraySkillDuration = 4f;

    [Header("����Ƶ��")]
    [Tooltip("Ball�������")]
    public float ballCastInterval = 1f;
    [Tooltip("Vine�������")]
    public float vineCastInterval = 1.2f;

    [Header("��������")]
    public float rayAttackRange = 40f;
    public float vineAttackRange = 10f;
    public float ballAttackRange = 35f;
    public float sprayAttackRange = 6f;
}