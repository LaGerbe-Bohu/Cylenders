using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.BeforeStack, "BOHU/Outlines")]
public class OutlinePP : PostProcessEffectSettings
{

    
    public FloatParameter NearClipPlaneDepth = new FloatParameter {value = .02f};
    public FloatParameter Scale = new FloatParameter {value = 1};
    public FloatParameter DepthThreshold = new FloatParameter {value = 30f};
    public FloatParameter NormalThreshold = new FloatParameter {value = 10f};
    public FloatParameter DepthNormalThresholdScale = new FloatParameter {value = .1f};
    public FloatParameter TextureThreshold = new FloatParameter {value = 10f};
    public ColorParameter ColorOutLine = new ColorParameter() {value = Color.black};
}

public class PostProcessOutlineRenderer : PostProcessEffectRenderer<OutlinePP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/BOHU/OutLine"));
        sheet.properties.SetFloat("_NearClipPlaneDepth",settings.NearClipPlaneDepth);
        sheet.properties.SetFloat("_Scale",settings.Scale);
        sheet.properties.SetFloat("_DepthThreshold",settings.DepthThreshold);
        sheet.properties.SetFloat("_DepthNormalThresholdScale",settings.DepthNormalThresholdScale);
        sheet.properties.SetFloat("_NormalThreshold",settings.NormalThreshold);
        sheet.properties.SetFloat("_TextureThreshold",settings.TextureThreshold);
        sheet.properties.SetColor("_Color",settings.ColorOutLine);
        Vector3 dir = Camera.current.transform.forward;
        sheet.properties.SetFloatArray("_CameraDirection",new List<float>(){dir.x,dir.y,dir.z});
        
        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;
        sheet.properties.SetMatrix("_ClipToView", clipToView);
        
        context.command.BlitFullscreenTriangle(context.source,context.destination,sheet,0);
    }
}

