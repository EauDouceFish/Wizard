using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 专门针对GameObject的对象池管理，预热+单例、类型安全、内存管理会把目标Parent设置到自己下方
/// 正在制作完善，暂时不可以使用
/// </summary>
public sealed class ObjectPool : MonoBehaviour
{
    [Serializable]
    public class StartupPool
    {
        public int size;
        public GameObject prefab;
    }
    // 维护初始化对象池
    public StartupPool[] startupPools;

    // 对象池使用字典维护记录了具体有哪些对象，按种类存放在List中。

    /// <summary>
    /// 维护已存储的对象：对应prefab-具体创建的对象List
    /// </summary>
    Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    /// <summary>
    /// 维护已生成的对象：具体创建的对象-对应prefab
    /// </summary>
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    static ObjectPool instance;

    // 标志是否已经创建初始对象池
    private bool startupPoolsCreated;

    private void Awake()
    {
        instance = this;
        CreateStartupPools();
    }
    // 单例对象池，为空则立刻创建
    public static ObjectPool Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            instance = FindObjectOfType<ObjectPool>();
            if (instance != null)
            {
                return instance;
            }
            else
            {
                GameObject pool = new GameObject("ObjectPool");
                pool.transform.localPosition = Vector3.zero;
                pool.transform.localRotation = Quaternion.identity;
                pool.transform.localScale = Vector3.one;
                instance = pool.AddComponent<ObjectPool>();
                return instance;
            }
        }
    }

    /// <summary>
    /// 创建对应prefab的对象池，如果已经存在就忽略即可
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="initialPoolSize"></param>
    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
        // 对象非空且还未保存，将实例化后对象设置为inactive，再存入对象池中
        if (prefab != null && !instance.pooledObjects.ContainsKey(prefab))
        {
            List<GameObject> prefabInstanceList = new List<GameObject>();
            instance.pooledObjects.Add(prefab, prefabInstanceList);
            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(active);
                Transform parent = instance.transform;
                // 初始n个对象进入对象池的对应List
                while (prefabInstanceList.Count < initialPoolSize)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.transform.parent = parent;
                    prefabInstanceList.Add(obj);
                }
            }
        }
    }

    /// <summary>
    /// 对所有要用对象池的Prefab都调用CreatePool
    /// 创建对象池的总函数之一
    /// </summary>
    public static void CreateStartupPools()
    {
        if (!instance.startupPoolsCreated)
        {
            instance.startupPoolsCreated = true;
            StartupPool[] startupPools = instance.startupPools;
        }
    }

    /// <summary>
    /// 传入prefab，parent，offset position，rotation以实例化对象
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        // 维护当前生成对象，在对象池中的列表相关信息
        List<GameObject> prefabInstanceList;
        // 维护当前对象的Transform信息
        Transform trans;
        // 维护当前对象
        GameObject obj;

        // 对象池有存储，从对象池中操作进行实例化
        // 对象池没存储，作为通用对象进行实例化
        if (instance.pooledObjects.TryGetValue(prefab, out prefabInstanceList))
        {
            obj = null;
            if (prefabInstanceList.Count > 0)
            {
                while (obj == null && prefabInstanceList.Count > 0)
                {
                    obj = prefabInstanceList[0];
                    prefabInstanceList.RemoveAt(0);
                }
                if (obj != null)
                {
                    trans = obj.transform;
                    trans.parent = parent;
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    instance.spawnedObjects.Add(obj, prefab);
                    return obj;
                }
            }
            // 对象池中没有找到合适数据，重新创建并加入管理
            obj = Instantiate(prefab);
            trans = obj.transform;
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            instance.spawnedObjects.Add(obj, prefab);
            return obj;
        }
        else
        {
            obj = Instantiate(prefab);
            trans = obj.transform;
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            instance.spawnedObjects.Add(obj, prefab);
            return obj;
        }
    }

    // 对象失活逻辑：放入pool存储，移除spawnedObjects数据，并放回对象池位置，失活
    private static void Recycle(GameObject obj, GameObject prefab)
    {
        instance.pooledObjects[prefab].Add(obj);
        instance.spawnedObjects.Remove(obj);
        obj.transform.parent = instance.transform;
        obj.SetActive(false);
    }

    /// <summary>
    /// 外部可用方法：试图回收指定GameObject入对象池
    /// </summary>
    /// <param name="obj"></param>
    public static void Recycle(GameObject obj)
    {
        GameObject prefab;
        if (instance.spawnedObjects.TryGetValue(obj, out prefab))
        {
            Recycle(obj, prefab);
        }
        else
        {
            Destroy(obj);
        }
    }

    /// <summary>
    /// 外部可用方法：从对象池试图生成任意类型T的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
    }


    /// <summary>
    /// 外部可用方法：试图回收任意类型T的指定GameObject入对象池
    /// </summary>
    /// <param name="obj"></param>
    public static void Recycle<T>(T obj) where T : Component
    {
        Recycle(obj.gameObject);
    }
}

public static class ObjectPoolExtensions
{
    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return ObjectPool.Spawn(prefab, parent, position, rotation);
    }

    public static void Recycle<T>(this T obj) where T : Component
    {
        ObjectPool.Recycle(obj);
    }

    public static void Recycle(this GameObject obj)
    {
        ObjectPool.Recycle(obj);
    }
}