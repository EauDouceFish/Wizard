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

    [Header("��ͼ��������")]
    [TextArea(3, 5)]
    public string miniMapDescription = "һ������{0}��Ⱥϵ����С�ĵ�ͼ���ʺϿ��ټ�����";
    [TextArea(3, 5)]
    public string smallMapDescription = "����{0}��ȺϵС�͵�ͼ�����Ԫ��ħ�������Ҳ����";
    [TextArea(3, 5)]
    public string mediumMapDescription = "����{0}��Ⱥϵ�������ͼ����ʼ��Ϸ��";
    [TextArea(3, 5)]
    public string largeMapDescription = "����{0}��Ⱥϵ���зǳ�������Ĵ��ͼ��������Ϸʱ�䳤";
}
