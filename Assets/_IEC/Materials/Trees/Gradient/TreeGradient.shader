Shader "Custom/Tree Gradient"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TopColor("Top Color", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
		_FogStart("Start", float) = 1.5
		_FogEnd("End", float) = .1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM

		#pragma surface surf Lambert finalcolor:mycolor vertex:myvert

		struct Input
		{
			float2 uv_MainTex;
			half fog;
		};

		fixed4 _Color;
		fixed4 _TopColor;
		half _FogStart;
		half _FogEnd;
		sampler2D _MainTex;

		void myvert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input,data);
			float4 pos = mul(unity_ObjectToWorld, v.vertex).xyzw;
			data.fog = saturate((_FogStart - pos.y) / (_FogStart - _FogEnd));
		}

		void mycolor(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			fixed3 botColor = _Color.rgb;
			fixed3 botColor2 = botColor * botColor * botColor;
			fixed3 topColor2 = botColor * _TopColor;

			#ifdef UNITY_PASS_FORWARDADD
			botColor = 0;
			#endif
			color.rgb = lerp(color.rgb * _TopColor, botColor2, IN.fog);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		}

		ENDCG
	}

	Fallback "Diffuse"
}