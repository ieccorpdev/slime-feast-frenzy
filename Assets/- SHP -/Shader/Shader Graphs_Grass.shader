Shader "Custom/GrassShader"
{
    Properties
    {
        [NoScaleOffset] _Diffuse ("Diffuse", 2D) = "white" {}
        _Tiling ("Tiling", Vector) = (1, 0, 0, 0)
        _Color ("Color", Color) = (0, 0, 0, 0)
        _StencilRef ("Stencil Reference", Float) = 1 
        _Scale ("UV Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "LightMode"="ForwardBase"}
        LOD 100

        Pass
        {
            Stencil
            {
                Ref 2  // Use the property value as the stencil reference
                Comp greater // Only render where the stencil buffer is equal to the reference value
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"
            // Define properties
            sampler2D _Diffuse;
            float4 _Tiling;
            float4 _Color;
            float _Scale;
            struct v2f
            {
                float2 worldUV : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Apply tiling to the UV coordinates
                 o.worldUV = worldPos.xz * _Scale;
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the diffuse texture
                fixed4 texColor = tex2D(_Diffuse, i.worldUV);
                // Multiply by the color property
                fixed shadow = SHADOW_ATTENUATION(i);
                return texColor * _Color * shadow;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
