using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OutlineControllerͨ���ڽ���ʱ�Զ����ù���
/// </summary>
public class OutlineController : MonoBehaviour, IController
{
    private static readonly int s_ShaderProp_OutlineColor = Shader.PropertyToID("_OutlineColor");
    [SerializeField] private Material outlineMaterial;
    [SerializeField] [ColorUsage(true, true)] private Color outlineColor;
    [SerializeField] private bool includeChildren = true; // ����Ƿ����������

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
            // ��ȡ����������������Renderer
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            allRenderers.AddRange(renderers);
        }
        else
        {
            // ֻ��ȡ�����Renderer
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
                // �ڵڶ�λ����1
                renderer.renderingLayerMask |= 1 << 1;
            }
            else
            {
                renderer.renderingLayerMask &= ~((uint)1 << 1);
            }
        }
    }

    /// <summary>
    /// �޸��Լ���������ɫ
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
