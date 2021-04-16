// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Stencils/StencilMask" {
	Properties {
		_StencilMask("Stencil Mask", Int) = 0
	}
    
    /*SubShader {
        Tags {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-100"
        }
		ZWrite on
		Stencil {
            Ref 0
            Comp always
        }
        
        Pass {
            Cull Front
            Color (1,0,0,1)
        }
    }*/
    SubShader {
        Tags {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-100"
        }
		ColorMask 0
        
		ZWrite on
		Stencil {
            Ref[_StencilMask]
            Comp always
            Pass replace
        }

        Pass {
            Cull Back
             
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : COLOR{
                return half4(1,1,0,1);
            }
            ENDCG
        }
    }
}
