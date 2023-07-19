using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessTransition), PostProcessEvent.BeforeStack, "BOHU/Scene Transition")]
public class SceneTransition : PostProcessEffectSettings
{
    public ColorParameter Color = new ColorParameter {value = UnityEngine.Color.black};
    [Range(0.001f, 1.0f)]
    public FloatParameter treshold = new FloatParameter {value = .01f};
    public FloatParameter noiseStrength = new FloatParameter {value = 2f};
    public TextureParameter NoiseTexture = new TextureParameter {value = null};}

public class PostProcessTransition : PostProcessEffectRenderer<SceneTransition>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/BOHU/smoothTransition"));
        sheet.properties.SetInt("InTransition",0);
        if (GameManager.instance.InAnimiantion)
        {
            sheet.properties.SetInt("InTransition",1);
        }

        sheet.properties.SetFloat("timeStep",GameManager.instance.timeStepAnimation);
        sheet.properties.SetColor("color", settings.Color);
        sheet.properties.SetFloat("treshold",settings.treshold);
        sheet.properties.SetFloat("noiseStrength",settings.noiseStrength);
        
        var imageTexture = settings.NoiseTexture.value == null? RuntimeUtilities.whiteTexture: settings.NoiseTexture.value;
        
        sheet.properties.SetTexture("noiseTexture",imageTexture);
        context.command.BlitFullscreenTriangle(context.source,context.destination,sheet,0);
    }
}

