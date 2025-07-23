using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
public static class ObjectPool
{
    private static readonly Dictionary<string, ObjectPool<object>> pool;
    /// <summary>
    /// 从指定的池中取出Object
    /// 如果无法转换为T类型会报错哦
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public static T Get<T>(string poolName) where T : class, new()
    {
        if (!pool.ContainsKey(poolName))
        {
            pool.Add(poolName, new ObjectPool<object>(() => { return new T(); }));
        }
        return (T)pool[poolName].Get();
    }
    /// <summary>
    /// 将Object放入指定的池中
    /// </summary>
    /// <param name="item"></param>
    /// <param name="poolName"></param>
    public static void Release(object item, string poolName)
    {
        if (!pool.ContainsKey(poolName))
        {
            return;
        }
        pool[poolName].Release(item);
    }
    static ObjectPool()
    {
        pool = new Dictionary<string, ObjectPool<object>>(10);
    }
}
