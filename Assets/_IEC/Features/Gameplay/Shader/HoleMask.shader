Shader "Thanhday/HoleMask"
{
    Properties
    {
        _StencilRef ("Stencil Reference", Float) = 1 
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        
        Pass
        {
            Stencil
            {
                Ref [_StencilRef]  
                Comp Always         
                Pass Replace        
            }
            Zwrite off
            ColorMask 0 
        }
    }
}
