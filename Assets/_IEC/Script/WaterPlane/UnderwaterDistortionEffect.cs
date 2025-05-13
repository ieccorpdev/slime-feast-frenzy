using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UnderwaterDistortionEffect : MonoBehaviour
{
    public Shader shader;
    public Texture2D distortionTex1;
    public Texture2D distortionTex2;
    public float strength = 0.03f;
    public float speed1 = 0.2f;
    public float speed2 = 0.15f;

    private Material _material;

    void Start()
    {
        if (shader == null)
            shader = Shader.Find("Hidden/UnderwaterMultiDistortion");

        if (shader != null)
            _material = new Material(shader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_material != null)
        {
            _material.SetTexture("_DistortionTex1", distortionTex1);
            _material.SetTexture("_DistortionTex2", distortionTex2);
            _material.SetFloat("_Strength", strength);
            _material.SetFloat("_Speed1", speed1);
            _material.SetFloat("_Speed2", speed2);

            Graphics.Blit(src, dest, _material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}