Shader "Custom/OmniShader"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
        _Tiling ("Tiling", Vector) = (1, 0, 0, 0)
        _Color ("Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Stencil
            {
                Ref 2  // Use the property value as the stencil reference
                Comp greater // Only render where the stencil buffer is equal to the reference value
            }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Define properties
            sampler2D _MainTex;
            float4 _Tiling;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Apply tiling to the UV coordinates
                o.uv = v.uv * _Tiling.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the diffuse texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // Multiply by the color property
                return texColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
