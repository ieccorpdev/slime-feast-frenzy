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
    private static readonly int DistortionTex1 = Shader.PropertyToID("_DistortionTex1");
    private static readonly int DistortionTex2 = Shader.PropertyToID("_DistortionTex2");
    private static readonly int Strength = Shader.PropertyToID("_Strength");
    private static readonly int Speed1 = Shader.PropertyToID("_Speed1");
    private static readonly int Speed2 = Shader.PropertyToID("_Speed2");
    public bool hasMaterial;
    void Start()
    {
        if (shader == null)
            shader = Shader.Find("Hidden/UnderwaterMultiDistortion");

        if (shader != null)
        {
            _material = new Material(shader);
            hasMaterial = true;
        }
        else
        {
            hasMaterial = true;
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (hasMaterial)
        {
            _material.SetTexture(DistortionTex1, distortionTex1);
            _material.SetTexture(DistortionTex2, distortionTex2);
            _material.SetFloat(Strength, strength);
            _material.SetFloat(Speed1, speed1);
            _material.SetFloat(Speed2, speed2);
            Graphics.Blit(src, dest, _material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}