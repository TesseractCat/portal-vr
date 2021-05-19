Shader "Custom/PortalNoise"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "transparent" { }
        _NoiseTex ("Noise Texture", 2D) = "white" { }
        
        _DissolveSpeed ("Dissolve Speed", Float) = 1.0
        
        _StencilMask("Stencil Mask", Int) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Int) = 8 //Default Always=8, Equal=3

        _PlanePos ("Plane Position", Vector) = (0,0,0)
        _PlaneDir ("Plane Direction (Normal)", Vector) = (0,0,0)
    }
    
    SubShader
    {
        Tags { "Queue"="Geometry"  "IgnoreProjector"="True"}
        LOD 100
            
		Stencil{
			Ref[_StencilMask]
			Comp[_StencilComp]
			Pass keep
			Fail keep
		}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO 
            };
            
            fixed4 _Color;
        
            float3 _PlanePos;
            float3 _PlaneDir;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            
            float _DissolveTime = -1;
            float _DissolveSpeed;

            float noise(float2 p){
                return tex2D(_NoiseTex, p);
            }

            v2f vert (appdata_full v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (dot(i.worldPos.xyz - (_PlanePos.xyz + _PlaneDir.xyz/1000.0), _PlaneDir.xyz) > 0)
                    clip(-1);
                
                //Dissolve
                if (_DissolveTime > 0) {
                    clip(noise(i.uv/2.0) - saturate(_Time.y - _DissolveTime) * _DissolveSpeed);
                }
                
                //Trippy noise
                float2 uv = i.uv * fixed2(0.05, 0.1);
                uv = uv + float2(noise(uv/4.5 + _Time.xx/40.0), noise(uv/4.5 - _Time.xx/40.0 + float2(999,404)));
                fixed4 n = noise(uv*5.0 + _Time.xy/20.0);
                n += noise(uv*7.0 - _Time.yx/30.0 + float2(500,90.25));
                n = pow(n * 2, 4) / 10;
                n = pow(noise(n.xy/50.0) * 5.0, 5) - (float4(1,1,1,0) * 0.2);
                n.a = 1;
                
                n = lerp(n, tex2D(_MainTex, i.uv) * _Color, tex2D(_MainTex, i.uv).a);
                
                return n * _Color;
            }
            ENDCG
        }
    }
}
