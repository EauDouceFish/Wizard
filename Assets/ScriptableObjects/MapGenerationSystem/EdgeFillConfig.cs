using UnityEngine;

[CreateAssetMenu(fileName = "EdgeFillConfig", menuName = "MapGenerationSystem/EdgeFillConfig")]
public class EdgeFillConfig : ScriptableObject
{
    [Header("��ʯ������ת�Ƕȡ�����ۼ���ת�Ƕ�")]
    [Range(0f, 15f)]
    public float singleRotationRange = 5f;             // ������ת�Ƕȷ�Χ
    [Range(0f, 30f)]
    public float cumulativeRotationRange = 15f;        // �ۼ���תƫ�Ʒ�Χ

    [Header("����������ؿ�������")]
    [Range(1, 10)]
    public int maxFillCount = 3;                       // �����������ؿ���

    [Header("���������������������Ҫ������С����")]
    [Range(0.1f, 2f)]
    public float minStretchRatio = 0.5f;               // ��С�������
    [Range(0.5f, 3f)]
    public float maxStretchRatio = 1.5f;               // ����������

    [Header("�������Ӵ������ڵ�ƫ����")]
    public float innerOffset = 0.2f;                   // �������Ӵ�����ƫ����

    [Header("ĩβ����С������")]
    public float minStretchCorrection = 0.3f;              // ĩβ����С������
}