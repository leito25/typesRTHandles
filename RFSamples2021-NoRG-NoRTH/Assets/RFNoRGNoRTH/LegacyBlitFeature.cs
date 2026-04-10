using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LegacyBlitFeature : ScriptableRendererFeature
{
    class LegacyBlitPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        public LegacyBlitPass(Material mat) => blitMaterial = mat;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //var cmd = CommandBufferPool.Get(nameof(LegacyBlitPass));
            var cmd = CommandBufferPool.Get("Legacy Blit");
            
            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.Blit(source, source, blitMaterial);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
    
    public Material blitMaterial;
    LegacyBlitPass pass;

    public override void Create()
    {
        pass = new LegacyBlitPass(blitMaterial);
        pass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => renderer.EnqueuePass(pass);
}
