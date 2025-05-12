// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Luceed Studio/Built-in Water"
{
	Properties
	{
		[Header(Colors)]_ColorA("Color A", Color) = (1,1,1,1)
		_ColorB("Color B", Color) = (1,1,1,1)
		[Header(Depth)][Toggle(_DEPTH_ON)] _Depth("Depth", Float) = 0
		_DepthFade("Depth Fade", Float) = 2
		[Header(Noise)]_NoiseTilingX("Noise Tiling X", Range( 0 , 20)) = 1
		_NoiseTilingY("Noise Tiling Y", Range( 0 , 20)) = 1
		_NoiseSpeedX("Noise Speed X", Range( 0 , 1)) = 1
		_NoiseSpeedY("Noise Speed Y", Range( 0 , 1)) = 1
		[Header(Intersection)][Toggle(_INTERSECTION_ON)] _Intersection("Intersection", Float) = 0
		_IntersectionColor("Intersection Color", Color) = (1,1,1,1)
		_IntersectionDistance("Intersection Distance", Range( 0 , 2)) = 1
		_IntersectionThreshold("Intersection Threshold", Range( 0 , 1)) = 1
		_IntersectionNoiseThreshold("Intersection Noise Threshold", Range( 0 , 1)) = 1
		_IntersectionSmoothing("Intersection Smoothing", Range( 0 , 1)) = 1
		[Header(Sparkles)][Toggle(_SPARKLES_ON)] _Sparkles("Sparkles", Float) = 0
		_SparklesColor("Sparkles Color", Color) = (1,1,1,1)
		_SparklesThreshold("Sparkles Threshold", Range( 0 , 1)) = 0
		_SparklesSmoothing("Sparkles Smoothing", Range( 0 , 1)) = 0
		_SparklesSpeed("Sparkles Speed", Float) = 1
		[Header(Refraction)][Toggle(_REFRACTION_ON)] _Refraction("Refraction", Float) = 0
		_RefractionStrength("Refraction Strength", Range( 0 , 1)) = 1
		_RefractionOpacity("Refraction Opacity", Range( 0 , 1)) = 1
		_RefractionColorOpacity("Refraction Color Opacity", Range( 0 , 1)) = 1
		[Header(Specular)][Toggle(_SPECULAR_ON)] _Specular("Specular", Float) = 0
		_SpecularSize("Specular Size", Range( 0 , 1)) = -0.7
		_SpecularThreshold1("Specular Threshold", Range( 0 , 1)) = -0.7
		_SpecularSmoothing1("Specular Smoothing", Range( 0.001 , 1)) = 1
		_SpecularOpacity1("Specular Opacity", Range( 0 , 1)) = 1
		[Header(Shadow)]_ShadowColor("Shadow Color", Color) = (0,0,0,0)
		_ShadowThreshold("Shadow Threshold", Range( -1 , 1)) = 0
		_ShadowSharpness("Shadow Smoothing", Range( 0.01 , 1)) = 1
		_ShadowOpacity("Shadow Opacity", Range( 0 , 1)) = 1
		_IndirectDiffuseContribution("Indirect Diffuse Contribution", Range( 0 , 1)) = 1
		[HDR][Header(Rim)]_RimColor("Rim Color", Color) = (1,1,1,0)
		_RimPower("Rim Power", Range( 0.01 , 1)) = 0.4
		_RimOffset("Rim Offset", Range( 0 , 1)) = 0.6
		_RimOpacity("Rim Opacity", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _INTERSECTION_ON
		#pragma shader_feature_local _REFRACTION_ON
		#pragma shader_feature_local _SPECULAR_ON
		#pragma shader_feature_local _SPARKLES_ON
		#pragma shader_feature_local _DEPTH_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float4 screenPos;
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

		uniform float _IndirectDiffuseContribution;
		uniform float _ShadowThreshold;
		uniform float _ShadowSharpness;
		uniform float4 _ShadowColor;
		uniform float _ShadowOpacity;
		uniform float4 _ColorA;
		uniform float4 _ColorB;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFade;
		uniform float4 _SparklesColor;
		uniform float _SparklesThreshold;
		uniform float _SparklesSpeed;
		uniform float _NoiseSpeedX;
		uniform float _NoiseSpeedY;
		uniform float _NoiseTilingX;
		uniform float _NoiseTilingY;
		uniform float _SparklesSmoothing;
		uniform float _SpecularThreshold1;
		uniform float _SpecularSize;
		uniform float _SpecularSmoothing1;
		uniform float _SpecularOpacity1;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _RefractionStrength;
		uniform float _RefractionOpacity;
		uniform float _RefractionColorOpacity;
		uniform float4 _IntersectionColor;
		uniform float _IntersectionThreshold;
		uniform float _IntersectionNoiseThreshold;
		uniform float _IntersectionDistance;
		uniform float _IntersectionSmoothing;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float _RimOpacity;
		uniform float4 _RimColor;


		float2 voronoihash71_g13( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi71_g13( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash71_g13( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float2 voronoihash68_g13( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi68_g13( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash68_g13( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


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
			float zero113_g14 = 0.0;
			float3 temp_cast_0 = (1.0).xxx;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			UnityGI gi102_g14 = gi;
			float3 diffNorm102_g14 = normalize( WorldNormalVector( i , ase_normWorldNormal ) );
			gi102_g14 = UnityGI_Base( data, 1, diffNorm102_g14 );
			float3 indirectDiffuse102_g14 = gi102_g14.indirect.diffuse + diffNorm102_g14 * 0.0001;
			float3 lerpResult40_g14 = lerp( temp_cast_0 , indirectDiffuse102_g14 , _IndirectDiffuseContribution);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_31_0_g14 = ( 1.0 - ( ( 1.0 - 0.0 ) * _WorldSpaceLightPos0.w ) );
			float4 temp_cast_3 = (temp_output_31_0_g14).xxxx;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult10_g14 = dot( ase_normWorldNormal , ase_worldlightDir );
			float NdotL11_g14 = dotResult10_g14;
			float LightAttenuation196_g14 = ase_lightAtten;
			float4 temp_cast_4 = (( saturate( ( ( NdotL11_g14 + _ShadowThreshold ) / _ShadowSharpness ) ) * LightAttenuation196_g14 )).xxxx;
			float4 lerpResult44_g14 = lerp( temp_cast_3 , max( temp_cast_4 , _ShadowColor ) , _ShadowOpacity);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth18_g13 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth18_g13 = abs( ( screenDepth18_g13 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			float4 lerpResult32_g13 = lerp( _ColorB , _ColorA , saturate( distanceDepth18_g13 ));
			#ifdef _DEPTH_ON
				float4 staticSwitch34_g13 = lerpResult32_g13;
			#else
				float4 staticSwitch34_g13 = _ColorA;
			#endif
			float time71_g13 = ( _SparklesSpeed * _Time.y );
			float2 voronoiSmoothId71_g13 = 0;
			float2 appendResult78_g13 = (float2(_NoiseSpeedX , _NoiseSpeedY));
			float2 appendResult76_g13 = (float2(( _NoiseTilingX / 6.0 ) , ( _NoiseTilingY / 6.0 )));
			float2 panner77_g13 = ( 1.0 * _Time.y * appendResult78_g13 + ((ase_worldPos).xz*appendResult76_g13 + 0.0));
			float2 coords71_g13 = panner77_g13 * 8.0;
			float2 id71_g13 = 0;
			float2 uv71_g13 = 0;
			float voroi71_g13 = voronoi71_g13( coords71_g13, time71_g13, id71_g13, uv71_g13, 0, voronoiSmoothId71_g13 );
			float smoothstepResult15_g13 = smoothstep( ( 1.0 - _SparklesThreshold ) , 1.0 , voroi71_g13);
			float4 lerpResult26_g13 = lerp( staticSwitch34_g13 , _SparklesColor , saturate( ( smoothstepResult15_g13 / _SparklesSmoothing ) ));
			#ifdef _SPARKLES_ON
				float4 staticSwitch59_g13 = lerpResult26_g13;
			#else
				float4 staticSwitch59_g13 = staticSwitch34_g13;
			#endif
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult21_g13 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult20_g13 = dot( ase_normWorldNormal , normalizeResult21_g13 );
			float smoothstepResult56_g13 = smoothstep( ( 1.0 - _SpecularSize ) , 1.0 , dotResult20_g13);
			float smoothstepResult48_g13 = smoothstep( ( 1.0 - _SpecularThreshold1 ) , 1.0 , ( voroi71_g13 * smoothstepResult56_g13 ));
			float4 lerpResult60_g13 = lerp( staticSwitch59_g13 , _SparklesColor , ( saturate( ( smoothstepResult48_g13 / _SpecularSmoothing1 ) ) * _SpecularOpacity1 ));
			#ifdef _SPECULAR_ON
				float4 staticSwitch66_g13 = lerpResult60_g13;
			#else
				float4 staticSwitch66_g13 = staticSwitch59_g13;
			#endif
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float time68_g13 = _Time.y;
			float2 voronoiSmoothId68_g13 = 0;
			float2 coords68_g13 = panner77_g13 * 8.0;
			float2 id68_g13 = 0;
			float2 uv68_g13 = 0;
			float voroi68_g13 = voronoi68_g13( coords68_g13, time68_g13, id68_g13, uv68_g13, 0, voronoiSmoothId68_g13 );
			float4 screenColor29_g13 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( _RefractionStrength * voroi68_g13 ) ).xy);
			float4 lerpResult37_g13 = lerp( float4( 0,0,0,1 ) , screenColor29_g13 , _RefractionOpacity);
			float4 lerpResult40_g13 = lerp( lerpResult37_g13 , ( _ColorB * lerpResult37_g13 ) , _RefractionColorOpacity);
			#ifdef _REFRACTION_ON
				float4 staticSwitch65_g13 = saturate( ( lerpResult40_g13 + staticSwitch66_g13 ) );
			#else
				float4 staticSwitch65_g13 = staticSwitch66_g13;
			#endif
			float smoothstepResult3_g13 = smoothstep( ( 1.0 - _IntersectionNoiseThreshold ) , 1.0 , voroi68_g13);
			float screenDepth10_g13 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth10_g13 = abs( ( screenDepth10_g13 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectionDistance ) );
			float smoothstepResult2_g13 = smoothstep( ( 1.0 - _IntersectionThreshold ) , 1.0 , ( smoothstepResult3_g13 + ( 1.0 - distanceDepth10_g13 ) ));
			float4 lerpResult61_g13 = lerp( staticSwitch65_g13 , _IntersectionColor , saturate( ( saturate( ( smoothstepResult2_g13 / _IntersectionSmoothing ) ) * _IntersectionColor.a ) ));
			#ifdef _INTERSECTION_ON
				float4 staticSwitch62_g13 = lerpResult61_g13;
			#else
				float4 staticSwitch62_g13 = staticSwitch65_g13;
			#endif
			float dotResult38_g14 = dot( ase_normWorldNormal , ase_worldViewDir );
			float NdotV138_g14 = dotResult38_g14;
			float temp_output_85_0_g14 = ( saturate( NdotL11_g14 ) * pow( ( 1.0 - saturate( ( NdotV138_g14 + _RimOffset ) ) ) , _RimPower ) * _RimOpacity );
			float3 LightColorFalloff80_g14 = ( ase_lightColor.rgb * LightAttenuation196_g14 );
			c.rgb = ( zero113_g14 + ( ( float4( ( lerpResult40_g14 * ase_lightColor.a * temp_output_31_0_g14 ) , 0.0 ) + ( float4( ase_lightColor.rgb , 0.0 ) * lerpResult44_g14 ) ) * float4( (staticSwitch62_g13).xyz , 0.0 ) ) + float4( ( temp_output_85_0_g14 * LightColorFalloff80_g14 * (_RimColor).rgb ) , 0.0 ) ).rgb;
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
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;10364,-1520.859;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Luceed Studio/Built-in Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;147;10016.29,-1273.772;Inherit;False;Func_Water;0;;13;ea40088849380d94b9540e7aa1dd8108;0;0;1;COLOR;0
WireConnection;0;13;147;0
ASEEND*/
//CHKSM=BD1EAA26316CA7DB8E686BF6ED31DF67FEA8FE4E