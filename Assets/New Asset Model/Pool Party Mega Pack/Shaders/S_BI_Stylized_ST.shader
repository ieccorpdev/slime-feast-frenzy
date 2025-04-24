// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Luceed Studio/Built-in Stylized ST"
{
	Properties
	{
		[Header(Base)]_BaseTint("Base Tint", Color) = (0.7529412,0.7529412,0.7529412,1)
		_BaseTexture("Base Texture", 2D) = "white" {}
		_TextureOpacity("Texture Opacity", Range( 0 , 1)) = 1
		[Header(Shadow)]_ShadowColor("Shadow Color", Color) = (0,0,0,0)
		[Toggle(_SHADOWTEXTURE_ON)] _ShadowTexture("Shadow Texture", Float) = 0
		_ShadowColorTexture("Shadow Color Texture", 2D) = "black" {}
		_ShadowThreshold("Shadow Threshold", Range( -1 , 1)) = 0
		_ShadowSharpness("Shadow Smoothing", Range( 0.01 , 1)) = 1
		_ShadowOpacity("Shadow Opacity", Range( 0 , 1)) = 1
		_IndirectDiffuseContribution("Indirect Diffuse Contribution", Range( 0 , 1)) = 1
		[HDR][Header(Specular)]_SpecularTint("Specular Tint", Color) = (1,1,1,1)
		_SpecularTexture("Specular Texture", 2D) = "white" {}
		_SpecularThreshold("Specular Threshold", Range( -1 , -0.5)) = -0.7
		_SpecularSmoothing("Specular Smoothing", Range( 0.001 , 1)) = 1
		_SpecularOpacity("Specular Opacity", Range( 0 , 1)) = 1
		[HDR][Header(Rim)]_RimColor("Rim Color", Color) = (1,1,1,0)
		_RimPower("Rim Power", Range( 0.01 , 1)) = 0.4
		_RimOffset("Rim Offset", Range( 0 , 1)) = 0.6
		_RimOpacity("Rim Opacity", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _SHADOWTEXTURE_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _SpecularTint;
		uniform sampler2D _SpecularTexture;
		uniform float4 _SpecularTexture_ST;
		uniform float _SpecularThreshold;
		uniform float _SpecularSmoothing;
		uniform float _SpecularOpacity;
		uniform float _IndirectDiffuseContribution;
		uniform float _ShadowThreshold;
		uniform float _ShadowSharpness;
		uniform float4 _ShadowColor;
		uniform sampler2D _ShadowColorTexture;
		uniform float4 _ShadowColorTexture_ST;
		uniform float _ShadowOpacity;
		uniform float4 _BaseTint;
		uniform sampler2D _BaseTexture;
		uniform float4 _BaseTexture_ST;
		uniform float _TextureOpacity;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float _RimOpacity;
		uniform float4 _RimColor;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 HighlightColor83_g76 = (_SpecularTint).rgb;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float LightAttenuation196_g76 = ase_lightAtten;
			float3 LightColorFalloff80_g76 = ( ase_lightColor.rgb * LightAttenuation196_g76 );
			float2 UV151_g76 = i.uv_texcoord;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult137_g76 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult141_g76 = dot( ase_normWorldNormal , normalizeResult137_g76 );
			float NdotLV140_g76 = dotResult141_g76;
			float temp_output_188_0_g76 = ( saturate( ( ( NdotLV140_g76 + _SpecularThreshold ) / _SpecularSmoothing ) ) * _SpecularOpacity );
			float3 temp_cast_1 = (1.0).xxx;
			UnityGI gi102_g76 = gi;
			float3 diffNorm102_g76 = normalize( WorldNormalVector( i , ase_normWorldNormal ) );
			gi102_g76 = UnityGI_Base( data, 1, diffNorm102_g76 );
			float3 indirectDiffuse102_g76 = gi102_g76.indirect.diffuse + diffNorm102_g76 * 0.0001;
			float3 lerpResult40_g76 = lerp( temp_cast_1 , indirectDiffuse102_g76 , _IndirectDiffuseContribution);
			float temp_output_31_0_g76 = ( 1.0 - ( ( 1.0 - 0.0 ) * _WorldSpaceLightPos0.w ) );
			float4 temp_cast_4 = (temp_output_31_0_g76).xxxx;
			float dotResult10_g76 = dot( ase_normWorldNormal , ase_worldlightDir );
			float NdotL11_g76 = dotResult10_g76;
			float4 temp_cast_5 = (( saturate( ( ( NdotL11_g76 + _ShadowThreshold ) / _ShadowSharpness ) ) * LightAttenuation196_g76 )).xxxx;
			float2 uv_ShadowColorTexture = i.uv_texcoord * _ShadowColorTexture_ST.xy + _ShadowColorTexture_ST.zw;
			#ifdef _SHADOWTEXTURE_ON
				float4 staticSwitch206_g76 = tex2D( _ShadowColorTexture, uv_ShadowColorTexture );
			#else
				float4 staticSwitch206_g76 = _ShadowColor;
			#endif
			float4 lerpResult44_g76 = lerp( temp_cast_4 , max( temp_cast_5 , staticSwitch206_g76 ) , _ShadowOpacity);
			float4 lerpResult133_g76 = lerp( _BaseTint , ( tex2D( _BaseTexture, (UV151_g76*_BaseTexture_ST.xy + _BaseTexture_ST.zw) ) * _BaseTint ) , _TextureOpacity);
			float dotResult38_g76 = dot( ase_normWorldNormal , ase_worldViewDir );
			float NdotV138_g76 = dotResult38_g76;
			float temp_output_85_0_g76 = ( saturate( NdotL11_g76 ) * pow( ( 1.0 - saturate( ( NdotV138_g76 + _RimOffset ) ) ) , _RimPower ) * _RimOpacity );
			c.rgb = ( float4( ( HighlightColor83_g76 * LightColorFalloff80_g76 * tex2D( _SpecularTexture, (UV151_g76*_SpecularTexture_ST.xy + _SpecularTexture_ST.zw) ).r * temp_output_188_0_g76 ) , 0.0 ) + ( ( float4( ( lerpResult40_g76 * ase_lightColor.a * temp_output_31_0_g76 ) , 0.0 ) + ( float4( ase_lightColor.rgb , 0.0 ) * lerpResult44_g76 ) ) * float4( (lerpResult133_g76).rgb , 0.0 ) ) + float4( ( temp_output_85_0_g76 * LightColorFalloff80_g76 * (_RimColor).rgb ) , 0.0 ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4643.73,-1289.146;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Luceed Studio/Built-in Stylized ST;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;158;4060.895,-1062.768;Inherit;False;Func_Stylized;0;;76;a155e7cce67663f41963c02c06ef3ce2;7,203,1,107,0,144,0,130,1,109,1,128,1,111,1;2;190;FLOAT2;0,0;False;108;FLOAT4;0,0,0,0;False;2;COLOR;0;FLOAT;106
WireConnection;0;13;158;0
ASEEND*/
//CHKSM=91FF3E446A7DAEFD68DDB4702C631B9ADD41B308