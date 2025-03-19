Shader "Custom/UnlitTransparentWithAlphaMapAndColorCheck"
{
    Properties
    {
        _MainTex ("Main Texture (RGB)", 2D) = "white" {}    // Texture chính
        _AlphaMap ("Alpha Map (RGBA)", 2D) = "white" {}     // Alpha map
        _Color ("Color Tint", Color) = (1,1,1,1)           // Màu chỉnh tay
        _TransparencyFactor ("Transparency Factor", Range(0, 1)) = 1.0 // Điều chỉnh alpha
        _ColorAlpha ("Alpha Check Color (RGB)", Color) = (0,0,0,1)     // Màu để kiểm tra trong Alpha Map
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;           // Texture chính
            sampler2D _AlphaMap;         // Alpha map
            float4 _Color;               // Tint màu
            float4 _ColorAlpha;          // Màu kiểm tra trong Alpha Map
            float _TransparencyFactor;   // Điều chỉnh alpha

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lấy màu từ texture chính
                fixed4 col = tex2D(_MainTex, i.uv);

                // Lấy màu từ alpha map
                fixed4 alphaMapColor = tex2D(_AlphaMap, i.uv);

                // Tính độ chênh lệch giữa màu alpha map và ColorAlpha
                float colorDifference = length(alphaMapColor.rgb - _ColorAlpha.rgb);

                // Kiểm tra nếu độ chênh lệch nhỏ (màu trùng với ColorAlpha)
                if (colorDifference < 0.01)
                {
                    // Nếu trùng, bỏ qua làm mờ (giữ nguyên alpha gốc)
                    col.a = col.a;
                }
                else
                {
                    // Nếu không trùng, áp dụng alpha từ alpha map và TransparencyFactor
                    col.a *= alphaMapColor.r * _TransparencyFactor;
                }

                // Áp dụng tint màu
                col *= _Color;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
