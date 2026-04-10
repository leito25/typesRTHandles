using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class RenderGraphBlitFeature : ScriptableRendererFeature
{
    class RenderGraphBlitPass : ScriptableRenderPass
    {
        Material blitMaterial;
        
        public RenderGraphBlitPass(Material mat) => blitMaterial = mat;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resources = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            var desc = renderGraph.GetTextureDesc(resources.cameraColor);
            desc.name = "_";
            desc.clearBuffer = false;
            
            TextureHandle temp = renderGraph.CreateTexture(desc);
            
            //Blit camera Color -> temp
            RenderGraphUtils.BlitMaterialParameters blitParams;
            blitParams = new(resources.cameraColor, temp, blitMaterial, 0);
            renderGraph.AddBlitPass(blitParams);
            
            //Blit temp -> cameraColor
            blitParams = new(temp, resources.cameraColor, blitMaterial, 1);
            renderGraph.AddBlitPass(blitParams);
        }
    }
    
    public Material blitMaterial;
    private RenderGraphBlitPass pass;

    public override void Create()
    {
        pass = new RenderGraphBlitPass(blitMaterial);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => renderer.EnqueuePass(pass);
}
