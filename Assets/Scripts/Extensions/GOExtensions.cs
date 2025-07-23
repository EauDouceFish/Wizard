using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GOExtensions
{
    /// <summary>
    /// ��ȡCanvas
    /// </summary>
    /// <returns></returns>
    public static GameObject GetCanvas()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene.");
        }
        return canvas;
    }

    /// <summary>
    /// ��GameObject�²��������岢�һ�ȡ�Է����
    /// </summary>
    public static T Find<T>(this GameObject parent, string name) where T : Component
    {
        var child = parent.transform.Find(name);
        if (child == null)
        {
            Debug.LogError($"������ {name} �������� {parent.name} ��");
            return null;
        }

        T component = child.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"{name} ��δ���� {typeof(T).Name} ���");
        }

        return component;
    }

    /// <summary>
    /// ֻ���� GameObject
    /// </summary>
    public static GameObject Find(this GameObject parent, string name)
    {
        var child = parent.transform.Find(name);
        if (child == null)
        {
            Debug.LogError($"������ {name} �������� {parent.name} ��");
            return null;
        }
        return child.gameObject;
    }

    /// <summary>
    /// �޸�GameObject�Ĳ��ʣ���Ҫ�� MeshRenderer
    /// </summary>
    /// <param name="newMaterial"></param>
    /// <param name="gameObject"></param>
    public static void SetMaterial(this GameObject gameObject, Material newMaterial)
    {
        // Ӧ�ò���
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = newMaterial;
        }
        else
        {
            Debug.LogWarning($"GameObject {gameObject.name} does not have a MeshRenderer component.");
        }
    }

    /// <summary>
    /// ����AABB��Χ�м���ģ��ƫ��λ�ã��������Χ�еײ�����
    /// </summary>
    public static Vector3 GetModelGeometryOffsetPos(this GameObject model)
    {
        Bounds bounds = model.GetComponent<MeshRenderer>().bounds;
        Vector3 offsetPoint = Vector3.up * (0.5f * bounds.size.y - bounds.center.y);
        return offsetPoint;
    }


    // ��ȡģ��AABB��Χ��
    public static Bounds GetModelBoundsAABB(this GameObject gameObject)
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        else
        {
            Debug.LogWarning($"GameObject {gameObject.name} does not have a MeshRenderer component.");
            return new Bounds(Vector3.zero, Vector3.one);
        }
    }

    /// <summary>
    /// ��ȡָ��XZλ�õ�Ground����λ��
    /// </summary>
    public static Vector3 GetGroundPosition(Vector3 targetXZ)
    {
        // ���Ϸ����������ҵ�Ground����
        float startHeight = targetXZ.y + 20f;
        Vector3 startPosition = new Vector3(targetXZ.x, startHeight, targetXZ.z);

        Ray ray = new Ray(startPosition, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 50f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.point;
            }
        }

        // ���û���ҵ�Ground������ԭʼλ�ã�������Ҫ������
        return targetXZ;
    }

}
