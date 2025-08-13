using UnityEngine;

[CreateAssetMenu(fileName = "DefaultDecorationDistributionConfig", menuName = "MapGenerationSystem/DecorationDistributionConfig")]
public class DecorationDistributionConfig : ScriptableObject
{
    [Header("ï�������ϡ��������������")]
    [Range(0, 10)] public int minDenseRegionCount = 0;  // ï���������� ����
    [Range(0, 10)] public int maxDenseRegionCount = 1;  // ï���������� ����
    [Range(0, 10)] public int minSparseRegionCount = 0; // ϡ���������� ����
    [Range(0, 10)] public int maxSparseRegionCount = 3; // ϡ���������� ����

    [Header("һ��װ��������")]
    [Range(0, 6)] public int minLevel1DecorationCount_Dense = 2; // ï������һ��װ�������� ����
    [Range(0, 6)] public int maxLevel1DecorationCount_Dense = 4; // ï������һ��װ�������� ����
    [Range(0, 6)] public int minLevel1DecorationCount_Sparse = 1; // ϡ������һ��װ�������� ����
    [Range(0, 6)] public int maxLevel1DecorationCount_Sparse = 2; // ϡ������һ��װ�������� ����

    [Header("����װ��������")]
    [Range(0, 6)] public int minLevel2DecorationCount_Dense = 2;    // ï���������װ�������� ����
    [Range(0, 6)] public int maxLevel2DecorationCount_Dense = 7;    // ï���������װ�������� ����
    [Range(0, 6)] public int minLevel2DecorationCount_Sparse = 2;   // ϡ���������װ�������� ����
    [Range(0, 6)] public int maxLevel2DecorationCount_Sparse = 4;   // ϡ���������װ�������� ����

    [Header("����ƫ����")]
    [Range(0, 20)] public float minOffsetLevel1 = 4f;   // һ��װ��������ƫ���� ����
    [Range(0, 20)] public float maxOffsetLevel1 = 8f;   // һ��װ��������ƫ���� ����

    [Range(0, 50)] public float minOffsetLevel2 = 4f;   // ����װ��������ƫ���� ����
    [Range(0, 50)] public float maxOffsetLevel2 = 10f; // ����װ��������ƫ���� ����
}
