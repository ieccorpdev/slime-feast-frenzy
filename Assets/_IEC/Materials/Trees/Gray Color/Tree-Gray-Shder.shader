Shader "Custom/Gray-Color" {
    Properties {
        _Smooth("Smooth", Color) = (1,1,1,1)
        _Color ("Main Color", Color) = (1,1,1,1)
        _GrayColor ("Main Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(1,2)) = 1
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GLOBALMaskPosition ("Mask", Vector) = (1, 1, 1, 1)
        _FillAmount ("_Fill Amount", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull off
        LOD 200
 
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //#pragma surface surf Standard fullforwardshadows addshadow
        #pragma surface surf SimpleSpecular fullforwardshadows
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 h = normalize (lightDir + viewDir);

            half diff = max (0, dot (s.Normal, lightDir));

            float nh = max (0, dot (s.Normal, h));
            float spec = pow (nh, 48.0);

            half4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
            c.a = s.Alpha;
            return c;
        }
 
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _GrayColor;
        fixed4 _Smooth;

        float3 _GLOBALMaskPosition;
        fixed _FillAmount;
        fixed _Glossiness;
 
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
 
        float random (float2 input) { 
            return frac(sin(dot(input, float2(12.9898,78.233)))* 43758.5453123);
        }
 
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half dist = distance(_GLOBALMaskPosition, IN.worldPos);
            half sphere = 1 - saturate((dist - _FillAmount) / 0.1);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Albedo *= (_Smooth * _Glossiness);
            if(dist > _FillAmount) o.Albedo = _GrayColor.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}