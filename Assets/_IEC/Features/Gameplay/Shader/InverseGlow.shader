Shader "Thanhday/InverseGlow"
{
    Properties
    {
        _Color ("Main Color (with Alpha)", Color) = (1, 1, 1, 1) // Add color with transparency
        _MainTex ("Base (RGB)", 2D) = "white" {}                 // Base texture (optional)
        _Power ("Power",Range(0,10)) = 2
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off              // Disable depth writing for transparency
            Blend SrcAlpha OneMinusSrcAlpha // Standard alpha blending

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Power;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);   // Sample the texture
                texColor.a = pow(texColor.a,_Power);
                return texColor * _Color;                  // Apply color with transparency
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}
