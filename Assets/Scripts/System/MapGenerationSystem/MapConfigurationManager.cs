using UnityEngine;

/// <summary>
/// ��ͼ���ù������������ڼܹ����³�ʼ��ʱ���ֵ�ͼ����
/// </summary>
public static class MapConfigurationManager
{
    private static MapSize selectedMapSize = MapSize.Default;

    public static void SetSelectedMapSize(MapSize mapSize)
    {
        selectedMapSize = mapSize;
        Debug.Log($"MapConfigurationManager: ��ͼ��С����Ϊ {mapSize}");
    }

    public static MapSize GetSelectedMapSize()
    {
        return selectedMapSize;
    }

    public static bool HasValidMapSize()
    {
        return selectedMapSize != MapSize.Default;
    }

    public static void Reset()
    {
        selectedMapSize = MapSize.Default;
        Debug.Log("MapConfigurationManager: ��ͼ����������");
    }
}