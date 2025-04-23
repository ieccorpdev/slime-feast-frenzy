Shader "Unlit/ThanhdayVipPro2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 2)) = 1.05
        _Saturation ("Saturation", Range(0, 2)) = 1.2
        _Contrast ("Contrast", Range(0, 2)) = 1.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // Main pass
        Pass
        {
            CGPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _Saturation;
            float _Contrast;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 color = tex2D(_MainTex, i.uv);
                color.rgb *= _Brightness;

                float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
                color.rgb = lerp(float3(gray, gray, gray), color.rgb, _Saturation);
                color.rgb = ((color.rgb - 0.5) * _Contrast) + 0.5;
                return color;
            }
            ENDCG
        }

        // Optional: ShadowCaster pass
        Pass
        {
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
