Shader "UI/TutorialScreenShader"
{
        Properties
    {
        _Color ("Overlay Color", Color) = (0,0,0,0.8)
        _HoleRect ("Hole Rect", Vector) = (0.4, 0.4, 0.6, 0.6)
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="False"
        }

        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            Tags { "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _HoleRect;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 rectMin = _HoleRect.xy;
                float2 rectMax = _HoleRect.zw;

                if (uv.x > rectMin.x && uv.x < rectMax.x && uv.y > rectMin.y && uv.y < rectMax.y)
                {
                    discard;
                }

                return _Color;
            }
            ENDCG
        }
    }
}
