Shader "VolumetricLine/SingleLine-LightSaber-Glow" {
    Properties {
        [NoScaleOffset] _MainTex ("Base (RGB)", 2D) = "white" {}
        _LineWidth ("Line Width", Range(0.01, 100)) = 1.0
        _LineScale ("Line Scale", Float) = 1.0
        _LightSaberFactor ("LightSaberFactor", Range(0.0, 1.0)) = 0.9
        [MaterialToggle] _UvBasedLightSaberFactor("UV-Based Light Saber Calculation (Anti-Aliased)", Int) = 0
        _Color ("Main Color", Color) = (1,1,1,1)
        _Glow ("Glow Intensity", Range(0, 5)) = 0.8
        _GlowWidth ("Glow Width Multiplier", Range(1, 8)) = 3.0
        _GlowAlpha ("Glow Alpha", Range(0, 1)) = 0.4
    }
    SubShader {
        Tags {
            "DisableBatching"="True"
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "ForceNoShadowCasting"="True"
            "PreviewType"="Plane"
        }
        LOD 200

        // Core pass: alpha blended for the main line
        Pass {
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Lighting Off

            CGPROGRAM
            #pragma glsl_no_auto_normalization
            #pragma vertex vert
            #pragma fragment frag

            #define LIGHT_SABER_MODE_ON
            #include "_SingleLineShader.cginc"
            ENDCG
        }

        // Glow pass: additive for the outer glow
        Pass {
            Cull Off
            ZWrite Off
            ZTest LEqual
            Blend One One
            Lighting Off

            CGPROGRAM
            #pragma glsl_no_auto_normalization
            #pragma vertex vert
            #pragma fragment frag

            #define LIGHT_SABER_MODE_ON
            #define GLOW_PASS
            #include "_SingleLineShader.cginc"
            ENDCG
        }
    }
    FallBack "Diffuse"
}
