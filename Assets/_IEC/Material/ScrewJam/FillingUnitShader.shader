Shader "Custom/FillingUnitShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0, 1)) = 1.0
        _BaseY ("Base Y", float) = 1.0
        _MaxHeight  ("Height", float) = 1.0
        _Color ("Color", Color) = (1,1,1,1)
        _BlendAmount ("Blend Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        //Tags {  "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            // Enable blending
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FillAmount;
            float _BaseY;
            float _MaxHeight;
            fixed4 _Color;
float _BlendAmount;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 normal = normalize(i.worldNormal);
                float diff = max(0, dot(normal, lightDir));

                
                float worldHeight = i.worldPos.y+_BaseY;
                float fillHeight = lerp(0, _MaxHeight, _FillAmount); // Assuming the object height ranges from -1 to 1 in world space
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                if (worldHeight > fillHeight)
                {
                    col = fixed4(0.2f* diff,0.2f* diff,0.2f* diff,0.9f) ;
                    //col = fixed4(0,0,0,1.0f) ;
                }
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}