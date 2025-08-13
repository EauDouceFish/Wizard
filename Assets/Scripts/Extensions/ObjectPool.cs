using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ר�����GameObject�Ķ���ع���Ԥ��+���������Ͱ�ȫ���ڴ������Ŀ��Parent���õ��Լ��·�
/// �����������ƣ���ʱ������ʹ��
/// </summary>
public sealed class ObjectPool : MonoBehaviour
{
    [Serializable]
    public class StartupPool
    {
        public int size;
        public GameObject prefab;
    }
    // ά����ʼ�������
    public StartupPool[] startupPools;

    // �����ʹ���ֵ�ά����¼�˾�������Щ���󣬰���������List�С�

    /// <summary>
    /// ά���Ѵ洢�Ķ��󣺶�Ӧprefab-���崴���Ķ���List
    /// </summary>
    Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    /// <summary>
    /// ά�������ɵĶ��󣺾��崴���Ķ���-��Ӧprefab
    /// </summary>
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    static ObjectPool instance;

    // ��־�Ƿ��Ѿ�������ʼ�����
    private bool startupPoolsCreated;

    private void Awake()
    {
        instance = this;
        CreateStartupPools();
    }
    // ��������أ�Ϊ�������̴���
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
    /// ������Ӧprefab�Ķ���أ�����Ѿ����ھͺ��Լ���
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="initialPoolSize"></param>
    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
        // ����ǿ��һ�δ���棬��ʵ�������������Ϊinactive���ٴ���������
        if (prefab != null && !instance.pooledObjects.ContainsKey(prefab))
        {
            List<GameObject> prefabInstanceList = new List<GameObject>();
            instance.pooledObjects.Add(prefab, prefabInstanceList);
            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(active);
                Transform parent = instance.transform;
                // ��ʼn������������صĶ�ӦList
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
    /// ������Ҫ�ö���ص�Prefab������CreatePool
    /// ��������ص��ܺ���֮һ
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
    /// ����prefab��parent��offset position��rotation��ʵ��������
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        // ά����ǰ���ɶ����ڶ�����е��б������Ϣ
        List<GameObject> prefabInstanceList;
        // ά����ǰ�����Transform��Ϣ
        Transform trans;
        // ά����ǰ����
        GameObject obj;

        // ������д洢���Ӷ�����в�������ʵ����
        // �����û�洢����Ϊͨ�ö������ʵ����
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
            // �������û���ҵ��������ݣ����´������������
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

    // ����ʧ���߼�������pool�洢���Ƴ�spawnedObjects���ݣ����Żض����λ�ã�ʧ��
    private static void Recycle(GameObject obj, GameObject prefab)
    {
        instance.pooledObjects[prefab].Add(obj);
        instance.spawnedObjects.Remove(obj);
        obj.transform.parent = instance.transform;
        obj.SetActive(false);
    }

    /// <summary>
    /// �ⲿ���÷�������ͼ����ָ��GameObject������
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
    /// �ⲿ���÷������Ӷ������ͼ������������T�Ķ���
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
    /// �ⲿ���÷�������ͼ������������T��ָ��GameObject������
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