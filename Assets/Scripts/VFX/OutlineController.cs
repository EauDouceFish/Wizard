using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OutlineController通常在交互时自动设置挂载
/// </summary>
public class OutlineController : MonoBehaviour, IController
{
    private static readonly int s_ShaderProp_OutlineColor = Shader.PropertyToID("_OutlineColor");
    [SerializeField] private Material outlineMaterial;
    [SerializeField] [ColorUsage(true, true)] private Color outlineColor;
    [SerializeField] private bool includeChildren = true; // 描边是否包含子物体

    private OutlineSystem outlineSystem;
    private List<Renderer> allRenderers = new List<Renderer>();
    //private Renderer gameObjectRenderer;

    private void Awake()
    {
        outlineSystem = this.GetSystem<OutlineSystem>();
        CollectRenderers();
        SetOutlineMaterial();
    }

    private void CollectRenderers()
    {
        allRenderers.Clear();

        if (includeChildren)
        {
            // 获取自身和所有子物体的Renderer
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            allRenderers.AddRange(renderers);
        }
        else
        {
            // 只获取自身的Renderer
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                allRenderers.Add(renderer);
            }
        }
    }

    public void SetOutlineMaterial()
    {
        outlineMaterial = outlineSystem.GetOutlineMaterial();
    }

    public void SetOutlineEnabled(bool outlineEnabled)
    {
        if(allRenderers.Count == 0) return;
        foreach (var renderer in allRenderers)
        {
            if (renderer == null) continue;

            if (outlineEnabled)
            {
                // 在第二位设置1
                renderer.renderingLayerMask |= 1 << 1;
            }
            else
            {
                renderer.renderingLayerMask &= ~((uint)1 << 1);
            }
        }
    }

    /// <summary>
    /// 修改自己的轮廓颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetOutlineColor(Color color)
    {
        if (outlineMaterial != null)
        {
            outlineMaterial.SetColor(s_ShaderProp_OutlineColor, color);
        }
        else
        {
            Debug.LogWarning("Outline material is not assigned.");
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameCore.Interface;
    }
}
