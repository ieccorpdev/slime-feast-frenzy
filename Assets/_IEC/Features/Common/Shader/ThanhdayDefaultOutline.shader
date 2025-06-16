// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Thanhday/Default_Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        _OutlineWidth ("OutlineWidth",Float) = 1
        _OutlineInnerWidth ("OutlineInnerWidth",Float) = 1
        _OutlineColor("Outline Color",Color) = (1,1,1,1)
        _Power("Outline Power",float) = 1
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float _OutlineWidth;
            float _OutlineInnerWidth;
            
            fixed4 _OutlineColor;
            fixed _Power;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
               
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                /*OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
(tex.xy * name##_ST.xy + name##_ST.zw)*/
                OUT.texcoord = v.texcoord.xy * _MainTex_ST.xy * 1 + _MainTex_ST.zw;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 gaussianBlur(fixed4 color, fixed4 blurSize, fixed2 uv)
            {
                float weights[5] = {0.227027, 0.316216, 0.070270, 0.070270, 0.035985};
                
                for (int y = -4; y <= 4; y++)
                {
                    float offset = y * _MainTex_TexelSize.y * blurSize;
                    color += tex2D(_MainTex, float2(uv.x, uv.y + offset)) * weights[abs(y)];
                }
                for (int x = -4; x <= 4; x++)
                {
                    float offset = x * _MainTex_TexelSize.x * blurSize;
                    color += tex2D(_MainTex, float2(uv.x + offset, uv.y)) * weights[abs(x)];
                }
                return color;
            }
            fixed4 inverse_gaussianBlur(fixed4 color, fixed4 blurSize, fixed2 uv)
            {
                float weights[5] = {0.227027, 0.316216, 0.070270, 0.070270, 0.035985};
                
                for (int y = -4; y <= 4; y++)
                {
                    float offset = y * _MainTex_TexelSize.y * blurSize;
                    color += (1-tex2D(_MainTex, float2(uv.x, uv.y + offset))) * weights[abs(y)];
                }
                for (int x = -4; x <= 4; x++)
                {
                    float offset = x * _MainTex_TexelSize.x * blurSize;
                    color += (1-tex2D(_MainTex, float2(uv.x + offset, uv.y))) * weights[abs(x)];
                }
                return color;
            }
            /*fixed4 outerStroke(fixed2 uv)
            {
                float2 offset =  _MainTex_TexelSize.xy * _OutlineWidth;
                float offsetX = offset.x;
                float offsetY = offset.y;
                float2 uv1 = uv + float2(offsetX,0);
                float2 uv2 = uv + float2(-offsetX,0);
                float2 uv3 = uv + float2(0,-offsetY);
                float2 uv4 = uv + float2(0,-offsetY);

                float2 uv5 = uv + float2(offsetX,offsetY);
                float2 uv6 = uv + float2(-offsetX,offsetY);
                float2 uv7 = uv + float2(-offsetX,-offsetY);
                float2 uv8 = uv + float2(offsetX,-offsetY);

                half4 outlineColor = (tex2D(_MainTex, uv1 ) + _TextureSampleAdd);
                outlineColor += (tex2D(_MainTex, uv2 ) + _TextureSampleAdd);
                outlineColor += (tex2D(_MainTex, uv3 ) + _TextureSampleAdd);
                outlineColor += (tex2D(_MainTex, uv4 ) + _TextureSampleAdd);

                outlineColor += (tex2D(_MainTex, uv5 ) + _TextureSampleAdd);
                outlineColor += (tex2D(_MainTex, uv6 ) + _TextureSampleAdd);
                outlineColor += (tex2D(_MainTex, uv7 ) + _TextureSampleAdd);
                outlineColor += (tex2D(_MainTex, uv8 ) + _TextureSampleAdd);
                
                return outlineColor;
            }
            fixed4 interOutline(fixed2 uv)
            {
                float2 offset =  _MainTex_TexelSize.xy * _OutlineWidth;
                float offsetX = offset.x;
                float offsetY = offset.y;
                float2 uv1 = uv + float2(offsetX,0);
                float2 uv2 = uv + float2(-offsetX,0);
                float2 uv3 = uv + float2(0,-offsetY);
                float2 uv4 = uv + float2(0,-offsetY);

                float2 uv5 = uv + float2(offsetX,offsetY);
                float2 uv6 = uv + float2(-offsetX,offsetY);
                float2 uv7 = uv + float2(-offsetX,-offsetY);
                float2 uv8 = uv + float2(offsetX,-offsetY);

                half4 outlineColor = 1-(tex2D(_MainTex, uv1 ) + _TextureSampleAdd);
                 outlineColor += 1-(tex2D(_MainTex, uv2 ) + _TextureSampleAdd);
                 outlineColor += 1-(tex2D(_MainTex, uv3 ) + _TextureSampleAdd);
                 outlineColor += 1- (tex2D(_MainTex, uv4 ) + _TextureSampleAdd);
                //
                 outlineColor += 1-(tex2D(_MainTex, uv5 ) + _TextureSampleAdd);
                 outlineColor += 1-(tex2D(_MainTex, uv6 ) + _TextureSampleAdd);
                 outlineColor += 1-(tex2D(_MainTex, uv7 ) + _TextureSampleAdd);
                 outlineColor += 1-(tex2D(_MainTex, uv8 ) + _TextureSampleAdd);
                return outlineColor;
            }*/
            
            fixed4 frag(v2f IN) : SV_Target
            {
                
                half4 color = (tex2D(_MainTex, IN.texcoord ) + _TextureSampleAdd);

                fixed4 outerGau = gaussianBlur(color,_OutlineWidth,IN.texcoord);
                fixed4 interGau = inverse_gaussianBlur(1-color,_OutlineInnerWidth,IN.texcoord);
                float outlineGau = pow(interGau.a * outerGau.a,_Power) ;
                //return outlineGau;
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                //color = color + outlineColor;
                /*fixed4 inStroke = interOutline(IN.texcoord);
                fixed4 outStroke = outerStroke(IN.texcoord);
                fixed outline = inStroke.a * outStroke.a;*/
                
                if(outlineGau > 0.5)
                {
                    return _OutlineColor;
                }else
                {
                    return color * IN.color;
                }
                
            }
        ENDCG
        }
    }
}