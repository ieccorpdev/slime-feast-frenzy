Shader "Custom/WaterWCP/Block" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
        _GrayColor ("Gray Color", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Float) = 0
        _Version ("Have Gray", Float) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 250

    CGPROGRAM
    #pragma surface surf Lambert noforwardadd

    sampler2D _MainTex;
    sampler2D _BumpMap;
    fixed4 _GrayColor;

    float3 _MaskPos;
    fixed _FillAmount;

    fixed _Version;

    struct Input {
        float2 uv_MainTex;
        float3 worldPos;
    };

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;
        o.Alpha = c.a;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

        half dist = distance(_MaskPos, IN.worldPos);
	    half sphere = 1 - saturate((dist - _FillAmount) / 0.01);

	    if(_Version == 1 && dist > _FillAmount) o.Albedo *= _GrayColor;
    }
    ENDCG
    }

    FallBack "Mobile/Diffuse"
}