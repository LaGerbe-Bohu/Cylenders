using UnityEngine;

[ExecuteInEditMode]
public class AssignShader : MonoBehaviour
{
    public Material mat;
    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, mat);
    }
}
