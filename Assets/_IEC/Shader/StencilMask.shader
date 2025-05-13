Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry" }
        ColorMask 0
        ZWrite Off

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass {}
    }
}
