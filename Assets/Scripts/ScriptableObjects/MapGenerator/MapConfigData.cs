using UnityEngine;

/// <summary>
/// ������Map��������
/// </summary>
[CreateAssetMenu(fileName = "NewMapConfigData", menuName = "MapGenerationSystem/MapConfigData")]
public class MapConfigData : ScriptableObject
{
    public string configName = "DefaultMapConfig";      // ��������

    // ���ӽ��£�Ĭ��Map�Ŀ�͸�
    public int MiniMapWidth = 700;                      
    public int MiniMapHeight = 500;
    public int SmallMapWidth = 1000;
    public int SmallMapHeight = 1000;
    public int MediumMapWidth = 1200;
    public int MediumMapHeight = 1200;
    public int LargeMapWidth = 1500;
    public int LargeMapHeight = 1500;

    // Map���֧�ֵ�Ⱥϵ����
    public int MaxBoimeSupportMini = 2;
    public int MaxBoimeSupportSmall = 3;
    public int MaxBoimeSupportMedium = 4;
    public int MaxBoimeSupportLarge = 5;
}
