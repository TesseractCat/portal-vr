Shader "Unlit/StencilBack"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-100"
        }
		ZWrite on
        Pass {
            Cull Front
            Color (0,0,0,1)
        }
    }
}
