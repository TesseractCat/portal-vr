Shader "CustomStandard/StandardStencilVertexLit" {
        Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Spec Color", Color) = (1,1,1,0)
        _Emission ("Emissive Color", Color) = (0,0,0,0)
        /*[PowerSlider(5.0)]*/ _Shininess ("Shininess", Range (0.1, 1)) = 0.7
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    
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
     
            fixed4 _Color;
            fixed4 _SpecColor;
            fixed4 _Emission;
     
            half _Shininess;
     
            sampler2D _MainTex;
            float4 _MainTex_ST;
        
            float3 _PlanePos;
            float3 _PlaneDir;
            
            float _FizzleTime = -1;
     
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
                fixed3 diff : COLOR;
                fixed3 spec : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO 
            };
     
            v2f vert (appdata_full v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.pos = UnityObjectToClipPos (v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv_MainTex = TRANSFORM_TEX (v.texcoord, _MainTex);
                
                o.diff = ShadeVertexLights(v.vertex, v.normal) * _Color;
                o.diff += _Emission.rgb;
                
                o.spec = 0;
                
                for (int i = 0; i < 0; i++) {
                    float3 normal = normalize(mul(UNITY_MATRIX_MV, float4(normalize(v.normal), 0.0)).xyz);
                    float3 lightDir = normalize(
                            unity_LightPosition[i] - mul(UNITY_MATRIX_MV, v.vertex));
                    //o.diff = max(dot(normal, lightDir), 0.01) * _Color + unity_AmbientSky;
                    
                    float3 viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex));
                    viewDir = mul(UNITY_MATRIX_MV, float4(viewDir, 0.0)).xyz;
                    float3 reflectDir = reflect(-lightDir, normal);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                    o.spec += spec * _Shininess * _SpecColor;
                }
     
                return o;
            }
     
            fixed4 frag (v2f i) : COLOR {
                if (dot(i.worldPos.xyz - (_PlanePos.xyz + _PlaneDir.xyz/1000.0), _PlaneDir.xyz) > 0)
                    clip(-1);
                
                fixed4 c;
     
                fixed4 mainTex = tex2D (_MainTex, i.uv_MainTex);
     
                c.rgb = (mainTex.rgb * i.diff + i.spec);
                
                if (_FizzleTime > 0) {
                    c.rgb *= fixed3(1,1,1) * (1 - saturate(_Time.y - _FizzleTime));
                }
     
                clip(mainTex.a - 0.9);
     
                return c;
            }
     
            ENDCG
        }
     
    }
     
    Fallback "VertexLit"
}
