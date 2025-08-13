using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// 2Dƽ�棬��������ϵ�����ڵ�����(0, 0, 1)
    /// </summary>
    public static Vector3 inVector = new Vector3(0, 0, 1);
    /// <summary>
    /// ����һ��Vector�Լ�xyzֵ�������޸�ָ��xyzֵ
    /// </summary>
    /// <param name="original"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(
            x ?? original.x,
            y ?? original.y,
            z ?? original.z
        );
    }

    public static Vector3Int With(this Vector3Int original, int? x = null, int? y = null, int? z = null)
    {
        return new Vector3Int(
            x ?? original.x,
            y ?? original.y,
            z ?? original.z
        );
    }

    public static Vector3Int ToVector3Int(this Vector3 original)
    {
        return new Vector3Int(
            Mathf.RoundToInt(original.x),
            Mathf.RoundToInt(original.y),
            Mathf.RoundToInt(original.z)
        );
    }

    public static Vector2 xy(this Vector3 original)
    {
        return new Vector2(original.x, original.y);
    }

    public static Vector2Int xy(this Vector3Int original)
    {
        return new Vector2Int(original.x, original.y);
    }
}
