Shader "Custom/ParticleStencil"
{
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }

        Pass {
            // Set Stencil Operation for first pass
            Stencil {
                Ref 1
                Comp Always
                Pass Replace // Replace stencil buffer with the ref value 1
            }
            
            // Enable alpha blending for transparency
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off  // Don't write to depth buffer (important for transparency)
            
            // Add Texture sampling here for your particles
            CGPROGRAM
            #pragma surface surf Lambert
            sampler2D _MainTex;
            struct Input {
                float2 uv_MainTex;
            };
            void surf(Input IN, inout SurfaceOutput o) {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
                o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
            }
            ENDCG
        }

        Pass {
            // Only render particles where the stencil buffer is 1 (i.e., previous pass set it to 1)
            Stencil {
                Ref 1
                Comp Equal
                Pass Keep
            }

            // Render the particles with necessary properties (e.g., color, texture)
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off  // Don't write to depth buffer (important for transparency)
            
            // Add Texture sampling for particles
            CGPROGRAM
            #pragma surface surf Lambert
            sampler2D _MainTex;
            struct Input {
                float2 uv_MainTex;
            };
            void surf(Input IN, inout SurfaceOutput o) {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
                o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
            }
            ENDCG
        }
    }

    // Fallback shader in case the main shader is not supported
    Fallback "Diffuse"
}