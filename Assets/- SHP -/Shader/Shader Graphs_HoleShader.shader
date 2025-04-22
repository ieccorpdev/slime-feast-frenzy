Shader "Shader Graphs/HoleShader" {
	Properties {
		_Edge ("Edge", Float) = 10
		_Thickness ("Thickness", Float) = -5
		_Border ("Border", Vector) = (1,1,1,0)
		_Base ("Base", Vector) = (0,0,0,1)
		_Occlusion ("Occlusion", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}