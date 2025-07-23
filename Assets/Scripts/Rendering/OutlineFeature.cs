using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    class OutlineRenderPass : ScriptableRenderPass
    {
        private static readonly List<ShaderTagId> s_ShaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
        };

        // 把Shader的"_OutlineMask"参数存储起来，要注意参数名匹配
        private static readonly int s_ShaderProp_OutlineMask = Shader.PropertyToID("_OutlineMask");
        private RTHandle m_OutlineMaskRT;
        private readonly Material m_OutlineMaterial;
        private readonly MaterialPropertyBlock m_MaterialPropertyBlock;
        private readonly FilteringSettings m_FilteringSettings;


        public OutlineRenderPass(Material outlineMaterial)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            m_OutlineMaterial = outlineMaterial;
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
            // 项目Outline的RenderingLayerMask为2，此处不可修改
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.all, renderingLayerMask: 2);
        }

        public void Dispose()
        {
            if (m_OutlineMaskRT != null)
            {
                m_OutlineMaskRT.Release();
                m_OutlineMaskRT = null;
            }
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            //Debug.Log("OutlineRenderPass OnCameraSetup");
            ResetTarget();

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 1;       // 外轮廓不需要很大抗锯齿
            desc.depthBufferBits = 0;
            desc.colorFormat = RenderTextureFormat.ARGB32; // 使用透明度判断
            RenderingUtils.ReAllocateIfNeeded(ref m_OutlineMaskRT, desc, name: "_OutlineMaskRT");
        }


        /// <summary>
        /// Draw all the objects that need the outline effect.
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Outline Command");
            cmd.SetRenderTarget(m_OutlineMaskRT);
            cmd.ClearRenderTarget(true, true, Color.clear); // 每一帧清空RT

            var drawingSettings = CreateDrawingSettings(s_ShaderTagIds, ref renderingData, SortingCriteria.None);
            var rendererListParams = new RendererListParams(renderingData.cullResults, drawingSettings, m_FilteringSettings);
            var list = context.CreateRendererList(ref rendererListParams);
            cmd.DrawRendererList(list);

            // Draw outline to camera
            cmd.SetRenderTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
            m_MaterialPropertyBlock.SetTexture(s_ShaderProp_OutlineMask, m_OutlineMaskRT);
            cmd.DrawProcedural(Matrix4x4.identity, m_OutlineMaterial, 0, MeshTopology.Triangles, 3, 1, m_MaterialPropertyBlock);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            //Debug.Log("OutlineRenderPass OnCameraCleanup");
        }
    }

    [SerializeField] private Material m_OutlineMaterial;

    private bool IsMaterialValid => m_OutlineMaterial && m_OutlineMaterial.shader && m_OutlineMaterial.shader.isSupported;

    OutlineRenderPass m_OutlineRenderPass;

    /// <inheritdoc/>
    public override void Create()
    {
        if (!IsMaterialValid) return;
        m_OutlineRenderPass = new OutlineRenderPass(m_OutlineMaterial);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_OutlineRenderPass == null) return;
        renderer.EnqueuePass(m_OutlineRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        m_OutlineRenderPass?.Dispose();
    }
}


