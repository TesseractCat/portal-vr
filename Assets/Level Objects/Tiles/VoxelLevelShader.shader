Shader "Unlit/VoxelLevelShader"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        
		_PlanePos ("Plane Position", Vector) = (0,0,0)
		_PlaneDir ("Plane Direction (Normal)", Vector) = (0,0,0)
        
		_PortalPos1 ("Portal 1 Position", Vector) = (0,0,0)
		_PortalRot1 ("Portal 1 Rotation", Vector) = (0,0,0)
        _PortalTime1 ("Portal 1 Time", Float) = 0.0
        
		_PortalPos2 ("Portal 2 Position", Vector) = (0,0,0)
		_PortalRot2 ("Portal 2 Rotation", Vector) = (0,0,0)
        _PortalTime2 ("Portal 1 Time", Float) = 0.0
        
		_StencilMask("Stencil Mask", Int) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Int) = 3 //Default Always=8, Equal = 3
    }
    SubShader
    {
        Tags { "RenderType"="Vertex" }
        LOD 100

		Stencil{
			Ref[_StencilMask]
			Comp [_StencilComp]
			Pass keep
			Fail keep
		}

        Pass
        {
            Tags { LightMode=Vertex}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 color : TEXCOORD2;
                float3 diff : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            
            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            float3 _PlanePos;
            float3 _PlaneDir;

            float3 _PortalPos1;
            float3 _PortalPos2;

            float4 _PortalRot1;
            float4 _PortalRot2;
            
            float _PortalTime1;
            float _PortalTime2;
            
            float4x4 _MatrixRotX1;
            float4x4 _MatrixRotX2;
            float4x4 _MatrixRotY1;
            float4x4 _MatrixRotY2;
            
            /*
            float3x3 matrixRotX(float theta) {
                float3x3 out_matrix = {
                    1, 0, 0,
                    0, cos(theta), -sin(theta),
                    0, sin(theta), cos(theta)
                };
                return out_matrix;
            }
            
            float3x3 matrixRotY(float theta) {
                float3x3 out_matrix = {
                    cos(theta), 0, sin(theta),
                    0, 1, 0,
                    -sin(theta), 0, cos(theta)
                };
                return out_matrix;
            }*/
            
            int discardOval(float3 pos, float3 center, float2 rot, float3x3 matrixRotX, float3x3 matrixRotY, float radius) {
                pos = pos - center;
                pos = mul(matrixRotY, pos);
                pos = mul(matrixRotX, pos);
                pos = pos * float3(1.0, 0.5, 5.0);
                if (length(pos) < radius)
                    return -1;
                return 1;
            }

            v2f vert (appdata_full v)
            {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                o.viewDir = ObjSpaceViewDir(v.vertex);
                
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                
                o.diff = ShadeVertexLights(v.vertex, v.normal);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (dot(i.worldPos.xyz - (_PlanePos.xyz + _PlaneDir.xyz/1000.0), _PlaneDir.xyz) > 0)
                    clip(-1);
                
                clip(discardOval(
                            i.worldPos.xyz, _PortalPos1, _PortalRot1.xy, (float3x3)_MatrixRotX1, (float3x3)_MatrixRotY1,
                            smoothstep(0.0, 0.25, _Time.y - _PortalTime1) * 0.5));
                clip(discardOval(
                            i.worldPos.xyz, _PortalPos2, _PortalRot2.xy, (float3x3)_MatrixRotX2, (float3x3)_MatrixRotY2,
                            smoothstep(0.0, 0.25, _Time.y - _PortalTime2) * 0.5));
                
                fixed4 col = tex2D (_MainTex, i.uv) * fixed4(i.diff.rgb,1) * _Color * saturate(pow(i.color, 0.45));
                
                return col;
            }
            ENDCG
        }
    }
}
