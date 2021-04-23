// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomStandard/StandardStencilParticle"
{
    Properties
    {
        _TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        
        _StencilMask("Stencil Mask", Int) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Int) = 8 //Default Always=8, Equal=3
            
        _PlanePos ("Plane Position", Vector) = (0,0,0)
        _PlaneDir ("Plane Direction (Normal)", Vector) = (0,0,0)
    }

    SubShader {
        Tags {"Queue"="Geometry+50"  "IgnoreProjector"="True"}
        LOD 100
     
        ZWrite On
        
		Stencil{
			Ref[_StencilMask]
			Comp[_StencilComp]
			Pass keep
			Fail keep
		}
     
        Pass {
            Tags { LightMode = Vertex } 
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
            #pragma multi_compile_instancing
     
            #include "UnityCG.cginc"
     
            fixed4 _TintColor;
     
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _NoiseTex;
        
            float3 _PlanePos;
            float3 _PlaneDir;
     
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
                fixed3 diff : COLOR;
                fixed3 spec : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO 
            };
            
            //https://github.com/gkjohnson/unity-dithered-transparency-shader
            float isDithered(float2 pos, float alpha) {
                pos *= _ScreenParams.xy;

                // Define a dither threshold matrix which can
                // be used to define how a 4x4 set of pixels
                // will be dithered
                float DITHER_THRESHOLDS[16] =
                {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };

                int index = (int(pos.x) % 4) * 4 + int(pos.y) % 4;
                return alpha - DITHER_THRESHOLDS[index];
            }
     
            v2f vert (appdata_full v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.pos = UnityObjectToClipPos (v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv_MainTex = TRANSFORM_TEX (v.texcoord, _MainTex);

                return o;
            }
     
            fixed4 frag (v2f i) : COLOR {
                if (dot(i.worldPos.xyz - (_PlanePos.xyz + _PlaneDir.xyz/1000.0), _PlaneDir.xyz) > 0)
                    clip(-1);
                
                fixed4 c;
     
                fixed4 mainTex = tex2D (_MainTex, i.uv_MainTex);
                
                i.screenPos /= i.screenPos.w;
                clip(isDithered(i.screenPos.xy, mainTex.a * _TintColor.a));
     
                c.rgb = (mainTex.rgb * _TintColor);
     
                return c;
            }
     
            ENDCG
        }
     
    }
     
    Fallback "VertexLit"
}
