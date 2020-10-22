// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Warm-Diffuse" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

CGPROGRAM

sampler2D _MainTex;
fixed4 _Color;

struct Input {
    float2 uv_MainTex;
};

#pragma surface surf SimpleLambert
  
half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
    half NdotL = dot (s.Normal, lightDir);
	half diff = NdotL * 0.2 + 0.8;
	atten = atten * 0.5 + 0.5;
	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
	c.a = s.Alpha;
	return c;
}

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

Fallback "Legacy Shaders/VertexLit"
}
