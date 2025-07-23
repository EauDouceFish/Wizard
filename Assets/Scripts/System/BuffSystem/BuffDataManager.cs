using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public static class BuffDataManager
{
    private static Dictionary<string, BuffData> buffDataDictionary;

    #region Public方法
    /// <summary>
    /// 获得BuffData
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static BuffData GetBuffData<T>() where T : class
    {
        if (buffDataDictionary == null)
        {
            buffDataDictionary = new Dictionary<string, BuffData>(5);
        }

        string name = typeof(T).Name;
        BuffData result;

        buffDataDictionary.TryGetValue(name, out result);

        if (result == null)
        {
            result = LoadBuffData(name);
            buffDataDictionary.Add(name, result);
        }

        return result;
    }
    /// <summary>
    /// 释放掉缓存的所有BuffData,并释放字典内存
    /// </summary>
    public static void Release()
    {
        buffDataDictionary = null;

    }
    /// <summary>
    /// 释放掉缓存的所有BuffData,不释放字典内存
    /// </summary>
    public static void Clear()
    {
        buffDataDictionary.Clear();
    }
    #endregion

    #region 内部方法

    private const string folder = "Config/BuffSystem";
    private static BuffData LoadBuffData(string name)
    {
        BuffData result;
        string path = System.IO.Path.Combine(folder, name);
        result = Resources.Load<BuffData>(path);
        if (result == null)
        {
            throw new Exception($"加载BuffData失败，加载路径：{path}");
        }
        return result;
    }
    #endregion
}