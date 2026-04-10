using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RTHandleBlitFeature : ScriptableRendererFeature
{
    class RTHandleBlitPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private RTHandle tempHandle;
        
        public RTHandleBlitPass(Material mat) =>  blitMaterial = mat;

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            
            // Allocate temp RTHandle at camera size (could downsample here)
            RenderingUtils.ReAllocateIfNeeded(ref tempHandle, desc, name: "_RTHandleTemp");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isPreviewCamera) return;
            var cmd = CommandBufferPool.Get("RTHandle Blit");
            RTHandle colorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            // Example horizontal blur
            Blitter.BlitCameraTexture(cmd, colorTarget, tempHandle, blitMaterial, 0);
            Blitter.BlitCameraTexture(cmd, tempHandle, colorTarget, blitMaterial, 1);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public void Dispose() => tempHandle?.Release();
    }

    public Material blitMaterial;
    RTHandleBlitPass pass;

    public override void Create()
    {
        pass = new RTHandleBlitPass(blitMaterial);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }
        
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => renderer.EnqueuePass(pass);
        
    protected override void Dispose(bool disposing) => pass.Dispose();
}
