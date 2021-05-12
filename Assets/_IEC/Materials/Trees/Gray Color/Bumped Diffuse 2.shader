Shader "Mobile/Bumped Diffuse 2" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
        _GrayColor ("Gray Color", Color) = (1,1,1,1)
        _GLOBALMaskPosition ("Mask", Vector) = (1, 1, 1, 1)
        _FillAmount ("Fill Amount", Float) = 0
        _HaveGray ("Have Gray", Float) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 250

    CGPROGRAM
    #pragma surface surf Lambert noforwardadd

    sampler2D _MainTex;
    sampler2D _BumpMap;
    fixed4 _GrayColor;

    float3 _GLOBALMaskPosition;
    fixed _FillAmount;

    fixed _HaveGray;

    struct Input {
        float2 uv_MainTex;
        float3 worldPos;
    };

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;
        o.Alpha = c.a;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

        half dist = distance(_GLOBALMaskPosition, IN.worldPos);
	    half sphere = 1 - saturate((dist - _FillAmount) / 0);

	    if(_HaveGray > .5 && dist > _FillAmount) o.Albedo = _GrayColor.rgb;
    }
    ENDCG
    }

    FallBack "Mobile/Diffuse"
}