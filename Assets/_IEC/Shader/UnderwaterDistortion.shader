Shader "Hidden/UnderwaterMultiDistortion"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DistortionTex1 ("Distortion Map 1", 2D) = "white" {}
        _DistortionTex2 ("Distortion Map 2", 2D) = "white" {}
        _Strength ("Distortion Strength", Float) = 0.03
        _Speed1 ("Scroll Speed 1", Float) = 0.2
        _Speed2 ("Scroll Speed 2", Float) = 0.15
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DistortionTex1;
            sampler2D _DistortionTex2;
            float _Strength;
            float _Speed1;
            float _Speed2;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv1 = i.uv + _Time.y * _Speed1 * float2(1, 0.5);
                float2 uv2 = i.uv + _Time.y * _Speed2 * float2(-0.6, 0.8);

                float2 distort1 = tex2D(_DistortionTex1, uv1).rg * 2.0 - 1.0;
                float2 distort2 = tex2D(_DistortionTex2, uv2).rg * 2.0 - 1.0;

                float2 finalDistort = (distort1 + distort2) * 0.5 * _Strength;
                float2 uv = i.uv + finalDistort;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
