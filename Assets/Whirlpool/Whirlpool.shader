Shader "Custom/AdvancedWaterWhirlpool"
{
Properties
{
_MainTex ("Main Texture", 2D) = "white" {}
_NormalMap ("Normal Map", 2D) = "bump" {}
[HDR]_DepthColor ("Depth Color", Color) = (0, 0.3, 0.5, 1)
[HDR]_SurfaceColor ("Surface Color", Color) = (0.1, 0.6, 0.8, 1)
_TwistStrength ("Twist Strength", Range(0, 10)) = 2
_RotationSpeed ("Rotation Speed", Range(0, 5)) = 1
_Depth ("Depth Intensity", Range(0, 1)) = 0.5
_SpecularStrength ("Specular Strength", Range(0, 1)) = 0.5
_WaveStrength ("Wave Strength", Range(0, 1)) = 0.5
_WaveSpeed ("Wave Speed", Range(0, 5)) = 1
}
SubShader
{
Tags { "RenderType"="Transparent" "Queue"="Transparent" }
LOD 200

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
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 worldPos : TEXCOORD1;
};

sampler2D _MainTex;
sampler2D _NormalMap;
float4 _DepthColor;
float4 _SurfaceColor;
float _TwistStrength;
float _RotationSpeed;
float _Depth;
float _SpecularStrength;
float _WaveStrength;
float _WaveSpeed;

v2f vert (appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    float2 center = float2(0.5, 0.5);
    float2 delta = i.uv - center;

    // Calculate twist angle
    float angle = _Time.y * _RotationSpeed + length(delta) * _TwistStrength;

    // Create rotation matrix
    float cosA = cos(angle);
    float sinA = sin(angle);
    float2x2 rotationMatrix = float2x2(cosA, -sinA, sinA, cosA);
    float2 twistedUV = mul(rotationMatrix, delta) + center;

    // Sample the main texture with twisted UV
    fixed4 baseColor = tex2D(_MainTex, twistedUV);

    // Add depth effect
    float depthFactor = 1.0 - smoothstep(0.0, 0.5, length(delta) * _Depth);
    fixed4 depthColor = lerp(baseColor, _DepthColor, depthFactor);

    // Add normal map for surface detail
    float3 normal = UnpackNormal(tex2D(_NormalMap, twistedUV));
    float3 lightDir = normalize(float3(0.5, 0.5, 1.0));
    float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);

    // Calculate specular highlight
    float3 reflection = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflection), 0.0), 16.0) * _SpecularStrength;

    // Final color mix
    fixed4 surfaceColor = lerp(_SurfaceColor, depthColor, depthFactor);
    fixed4 finalColor = surfaceColor + spec;

    return finalColor;
}

ENDCG
}
}
}