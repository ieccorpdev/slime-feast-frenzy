Shader "Thanhday/GroundUnlitColor"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1) // This property allows you to choose a color in the editor
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Stencil
            {
                Ref 2  // Use the property value as the stencil reference
                Comp greater // Only render where the stencil buffer is equal to the reference value
            }
        Pass
        {
            // No lighting or texture sampling
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            // Vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // Fragment shader (color output)
            fixed4 _Color;

            fixed4 frag () : SV_Target
            {
                return _Color; // Output the color specified in the editor
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}