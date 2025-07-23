using QFramework;
using System;
using UnityEngine;

/// <summary>
/// OutlineControllerͨ���ڽ���ʱ�Զ����ù���
/// </summary>
[RequireComponent(typeof(Renderer))]
public class OutlineController : MonoBehaviour, IController
{
    private static readonly int s_ShaderProp_OutlineColor = Shader.PropertyToID("_OutlineColor");
    [SerializeField] private Material outlineMaterial;
    [SerializeField] [ColorUsage(true, true)] private Color outlineColor;
    private OutlineSystem outlineSystem;
    private Renderer gameObjectRenderer;

    private void Awake()
    {
        outlineSystem = this.GetSystem<OutlineSystem>();
        gameObjectRenderer = GetComponent<Renderer>();
        SetOutlineMaterial();
    }

    public void SetOutlineMaterial()
    {
        outlineMaterial = outlineSystem.GetOutlineMaterial();
    }

    public void SetOutlineEnabled(bool outlineEnabled)
    {
        if (outlineEnabled)
        {
            // �ڶ�λ����1
            gameObjectRenderer.renderingLayerMask |= 1 << 1;
        }
        else
        {
            gameObjectRenderer.renderingLayerMask &= ~((uint)1 << 1);
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
