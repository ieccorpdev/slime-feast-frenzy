// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Luceed Studio/Built-in Glass"
{
	Properties
	{
		[Header(Opacity)]_GlobalOpacity("Global Opacity", Range( 0 , 1)) = 0.5
		[Header(Fresnel)]_FresnelScale("Fresnel Scale", Range( 0 , 3)) = 1
		_FresnelPower("Fresnel Power", Range( 0 , 5)) = 1
		_FresnelBaseOpacity("Fresnel Base Opacity", Range( 0 , 1)) = 0.5
		_FresnelEdgeOpacity("Fresnel Edge Opacity", Range( 0 , 1)) = 0.5
		[Header(Bottom)]_BottomPosition("Bottom Position", Range( -2 , 2)) = 0.5
		_BottomThreshold("Bottom Threshold", Range( 0 , 1)) = 0.5
		_BottomSmoothing("Bottom Smoothing", Range( 0 , 1)) = 0.5
		_BottomOpacity("Bottom Opacity", Range( 0 , 1)) = 0.5
		[Header(HSV Pass)][Toggle(_HSVPASS_ON)] _HSVPass("HSV Pass", Float) = 0
		_AddSaturation("Add Saturation", Range( -1 , 1)) = 0
		_AddValue("Add Value", Range( -1 , 1)) = 0
		[Toggle(_HSVSATURATE_ON)] _HSVSaturate("HSV Saturate", Float) = 0
		[Header(Base)]_BaseTint("Base Tint", Color) = (0.7529412,0.7529412,0.7529412,1)
		_BaseTexture("Base Texture", 2D) = "white" {}
		_TextureOpacity("Texture Opacity", Range( 0 , 1)) = 1
		[Header(Shadow)]_ShadowColor("Shadow Color", Color) = (0,0,0,0)
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
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		GrabPass{ }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _HSVPASS_ON
		#pragma shader_feature_local _HSVSATURATE_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
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

		uniform float _BottomThreshold;
		uniform float _BottomPosition;
		uniform float _BottomSmoothing;
		uniform float _BottomOpacity;
		uniform float _FresnelBaseOpacity;
		uniform float _FresnelEdgeOpacity;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _GlobalOpacity;
		uniform float _SpecularThreshold;
		uniform float _SpecularSmoothing;
		uniform float _SpecularOpacity;
		uniform float4 _BaseTint;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float _RimOpacity;
		uniform float4 _SpecularTint;
		uniform sampler2D _SpecularTexture;
		uniform float4 _SpecularTexture_ST;
		uniform float _IndirectDiffuseContribution;
		uniform float _ShadowThreshold;
		uniform float _ShadowSharpness;
		uniform float4 _ShadowColor;
		uniform float _ShadowOpacity;
		uniform sampler2D _BaseTexture;
		uniform float4 _BaseTexture_ST;
		uniform float _TextureOpacity;
		uniform float4 _RimColor;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _AddSaturation;
		uniform float _AddValue;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
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
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float smoothstepResult160 = smoothstep( 0.0 , ( 1.0 - _BottomThreshold ) , ( ( 1.0 - ase_vertex3Pos.y ) + _BottomPosition ));
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV153 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode153 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV153, _FresnelPower ) );
			float lerpResult172 = lerp( _FresnelBaseOpacity , _FresnelEdgeOpacity , saturate( fresnelNode153 ));
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 normalizeResult137_g71 = normalize( ( ase_worldlightDir + ase_worldViewDir ) );
			float dotResult141_g71 = dot( ase_normWorldNormal , normalizeResult137_g71 );
			float NdotLV140_g71 = dotResult141_g71;
			float temp_output_188_0_g71 = ( saturate( ( ( NdotLV140_g71 + _SpecularThreshold ) / _SpecularSmoothing ) ) * _SpecularOpacity );
			float dotResult10_g71 = dot( ase_normWorldNormal , ase_worldlightDir );
			float NdotL11_g71 = dotResult10_g71;
			float dotResult38_g71 = dot( ase_normWorldNormal , ase_worldViewDir );
			float NdotV138_g71 = dotResult38_g71;
			float temp_output_85_0_g71 = ( saturate( NdotL11_g71 ) * pow( ( 1.0 - saturate( ( NdotV138_g71 + _RimOffset ) ) ) , _RimPower ) * _RimOpacity );
			float lerpResult177 = lerp( ( saturate( ( ( ( smoothstepResult160 / _BottomSmoothing ) * _BottomOpacity ) + lerpResult172 ) ) * _GlobalOpacity ) , 1.0 , saturate( ( temp_output_188_0_g71 + _BaseTint.a + temp_output_85_0_g71 ) ));
			float3 HighlightColor83_g71 = (_SpecularTint).rgb;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float LightAttenuation196_g71 = ase_lightAtten;
			float3 LightColorFalloff80_g71 = ( ase_lightColor.rgb * LightAttenuation196_g71 );
			float2 UV151_g71 = i.uv_texcoord;
			float3 temp_cast_1 = (1.0).xxx;
			UnityGI gi102_g71 = gi;
			float3 diffNorm102_g71 = normalize( WorldNormalVector( i , ase_normWorldNormal ) );
			gi102_g71 = UnityGI_Base( data, 1, diffNorm102_g71 );
			float3 indirectDiffuse102_g71 = gi102_g71.indirect.diffuse + diffNorm102_g71 * 0.0001;
			float3 lerpResult40_g71 = lerp( temp_cast_1 , indirectDiffuse102_g71 , _IndirectDiffuseContribution);
			float temp_output_31_0_g71 = ( 1.0 - ( ( 1.0 - 0.0 ) * _WorldSpaceLightPos0.w ) );
			float4 temp_cast_4 = (temp_output_31_0_g71).xxxx;
			float4 temp_cast_5 = (( saturate( ( ( NdotL11_g71 + _ShadowThreshold ) / _ShadowSharpness ) ) * LightAttenuation196_g71 )).xxxx;
			float4 lerpResult44_g71 = lerp( temp_cast_4 , max( temp_cast_5 , _ShadowColor ) , _ShadowOpacity);
			float4 lerpResult133_g71 = lerp( _BaseTint , ( tex2D( _BaseTexture, (UV151_g71*_BaseTexture_ST.xy + _BaseTexture_ST.zw) ) * _BaseTint ) , _TextureOpacity);
			float3 temp_output_194_0 = (( float4( ( HighlightColor83_g71 * LightColorFalloff80_g71 * tex2D( _SpecularTexture, (UV151_g71*_SpecularTexture_ST.xy + _SpecularTexture_ST.zw) ).r * temp_output_188_0_g71 ) , 0.0 ) + ( ( float4( ( lerpResult40_g71 * ase_lightColor.a * temp_output_31_0_g71 ) , 0.0 ) + ( float4( ase_lightColor.rgb , 0.0 ) * lerpResult44_g71 ) ) * float4( (lerpResult133_g71).rgb , 0.0 ) ) + float4( ( temp_output_85_0_g71 * LightColorFalloff80_g71 * (_RimColor).rgb ) , 0.0 ) )).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor180 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPos.xy/ase_grabScreenPos.w);
			float3 hsvTorgb181 = RGBToHSV( screenColor180.rgb );
			float3 hsvTorgb182 = HSVToRGB( float3(hsvTorgb181.x,( hsvTorgb181.y + _AddSaturation ),( hsvTorgb181.z + _AddValue )) );
			#ifdef _HSVSATURATE_ON
				float3 staticSwitch196 = saturate( hsvTorgb182 );
			#else
				float3 staticSwitch196 = hsvTorgb182;
			#endif
			float3 lerpResult185 = lerp( staticSwitch196 , temp_output_194_0 , lerpResult177);
			#ifdef _HSVPASS_ON
				float3 staticSwitch189 = lerpResult185;
			#else
				float3 staticSwitch189 = temp_output_194_0;
			#endif
			c.rgb = staticSwitch189;
			c.a = lerpResult177;
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
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
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
Node;AmplifyShaderEditor.FresnelNode;153;3823.719,-1277.346;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;155;4079.309,-1275.897;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;160;4093.879,-1583.197;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;162;3706.826,-1443.586;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;3367.623,-1445.185;Inherit;False;Property;_BottomThreshold;Bottom Threshold;6;0;Create;True;0;0;0;False;0;False;0.5;0.343;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;3875.593,-1657.986;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;166;3467.682,-1794.03;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;158;3193.056,-1788.019;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;164;3995.673,-1449.68;Inherit;False;Property;_BottomSmoothing;Bottom Smoothing;7;0;Create;True;0;0;0;False;0;False;0.5;0.339;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;4881.198,-1307.383;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;165;5087.951,-1310.179;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;4282.778,-1374.214;Inherit;False;Property;_BottomOpacity;Bottom Opacity;8;0;Create;True;0;0;0;False;0;False;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;172;4559.83,-1283.785;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;173;4103.022,-1173.079;Inherit;False;Property;_FresnelBaseOpacity;Fresnel Base Opacity;3;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;4104.498,-1095.443;Inherit;False;Property;_FresnelEdgeOpacity;Fresnel Edge Opacity;4;0;Create;True;0;0;0;False;0;False;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;3396.873,-1265.295;Inherit;False;Property;_FresnelScale;Fresnel Scale;1;1;[Header];Create;True;1;Fresnel;0;0;False;0;False;1;1.93;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;157;3415.938,-1156.947;Inherit;False;Property;_FresnelPower;Fresnel Power;2;0;Create;True;0;0;0;False;0;False;1;1.98;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;3367.787,-1533.315;Inherit;False;Property;_BottomPosition;Bottom Position;5;1;[Header];Create;True;1;Bottom;0;0;False;0;False;0.5;-0.78;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;175;4967.615,-1161.513;Inherit;False;Property;_GlobalOpacity;Global Opacity;0;1;[Header];Create;True;1;Opacity;0;0;False;0;False;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;5371.494,-1186.144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;163;4408.448,-1567.352;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;4660.14,-1472.225;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;177;5635.98,-1181.351;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;181;4668.092,-771.3841;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;184;4964.611,-659.0323;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;192;4964.279,-545.8116;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;194;5320.464,-998.4025;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;152;4800.408,-995.7814;Inherit;False;Func_Stylized;13;;71;a155e7cce67663f41963c02c06ef3ce2;7,203,0,107,0,144,0,130,1,109,1,128,1,111,1;3;204;SAMPLER2D;0,0,0,0;False;190;FLOAT2;0,0;False;108;FLOAT4;0,0,0,0;False;2;COLOR;0;FLOAT;106
Node;AmplifyShaderEditor.RangedFloatNode;190;4559.013,-537.2623;Inherit;False;Property;_AddSaturation;Add Saturation;10;0;Create;True;0;0;0;False;0;False;0;0.626;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;4565.334,-415.9009;Inherit;False;Property;_AddValue;Add Value;11;0;Create;True;0;0;0;False;0;False;0;0.737;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;182;5247.962,-747.3323;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;185;6308.549,-752.1595;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;6926.561,-1378.716;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Luceed Studio/Built-in Glass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.StaticSwitch;189;6543.863,-1002.178;Inherit;False;Property;_HSVPass;HSV Pass;9;0;Create;True;0;0;0;False;1;Header(HSV Pass);False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;196;5858.133,-747.7907;Inherit;False;Property;_HSVSaturate;HSV Saturate;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;195;5598.568,-672.8041;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenColorNode;180;4439.532,-771.3841;Inherit;False;Global;_GrabScreen0;Grab Screen 0;10;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;153;2;156;0
WireConnection;153;3;157;0
WireConnection;155;0;153;0
WireConnection;160;0;168;0
WireConnection;160;2;162;0
WireConnection;162;0;161;0
WireConnection;168;0;166;0
WireConnection;168;1;167;0
WireConnection;166;0;158;2
WireConnection;159;0;179;0
WireConnection;159;1;172;0
WireConnection;165;0;159;0
WireConnection;172;0;173;0
WireConnection;172;1;174;0
WireConnection;172;2;155;0
WireConnection;176;0;165;0
WireConnection;176;1;175;0
WireConnection;163;0;160;0
WireConnection;163;1;164;0
WireConnection;179;0;163;0
WireConnection;179;1;171;0
WireConnection;177;0;176;0
WireConnection;177;2;152;106
WireConnection;181;0;180;0
WireConnection;184;0;181;2
WireConnection;184;1;190;0
WireConnection;192;0;181;3
WireConnection;192;1;191;0
WireConnection;194;0;152;0
WireConnection;182;0;181;1
WireConnection;182;1;184;0
WireConnection;182;2;192;0
WireConnection;185;0;196;0
WireConnection;185;1;194;0
WireConnection;185;2;177;0
WireConnection;0;9;177;0
WireConnection;0;13;189;0
WireConnection;189;1;194;0
WireConnection;189;0;185;0
WireConnection;196;1;182;0
WireConnection;196;0;195;0
WireConnection;195;0;182;0
ASEEND*/
//CHKSM=08D7545302122BD21CDC2832404BEAB8A2DBDB01