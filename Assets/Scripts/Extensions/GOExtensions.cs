using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GOExtensions
{
    /// <summary>
    /// 获取Canvas
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
    /// 在GameObject下查找子物体并且获取对方组件
    /// </summary>
    public static T Find<T>(this GameObject parent, string name) where T : Component
    {
        var child = parent.transform.Find(name);
        if (child == null)
        {
            Debug.LogError($"子物体 {name} 不存在于 {parent.name} 下");
            return null;
        }

        T component = child.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"{name} 上未挂载 {typeof(T).Name} 组件");
        }

        return component;
    }

    /// <summary>
    /// 只查找 GameObject
    /// </summary>
    public static GameObject Find(this GameObject parent, string name)
    {
        var child = parent.transform.Find(name);
        if (child == null)
        {
            Debug.LogError($"子物体 {name} 不存在于 {parent.name} 下");
            return null;
        }
        return child.gameObject;
    }

    /// <summary>
    /// 修改GameObject的材质，需要有 MeshRenderer
    /// </summary>
    /// <param name="newMaterial"></param>
    /// <param name="gameObject"></param>
    public static void SetMaterial(this GameObject gameObject, Material newMaterial)
    {
        // 应用材质
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
    /// 根据AABB包围盒计算模型偏移位置，返回其包围盒底部中心
    /// </summary>
    public static Vector3 GetModelGeometryOffsetPos(this GameObject model)
    {
        Bounds bounds = model.GetComponent<MeshRenderer>().bounds;
        Vector3 offsetPoint = Vector3.up * (0.5f * bounds.size.y - bounds.center.y);
        return offsetPoint;
    }


    // 获取模型AABB包围盒
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
    /// 获取指定XZ位置的Ground表面位置
    /// </summary>
    public static Vector3 GetGroundPosition(Vector3 targetXZ)
    {
        // 从上方发射射线找到Ground表面
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

        // 如果没有找到Ground，返回原始位置（可能需要调整）
        return targetXZ;
    }

}
