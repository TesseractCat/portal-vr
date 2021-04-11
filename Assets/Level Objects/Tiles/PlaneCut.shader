Shader "Custom/PlaneCut" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_PlanePos ("Plane Position", Vector) = (0,0,0)
		_PlaneDir ("Plane Direction (Euler)", Vector) = (0,0,0)
        
		_PortalPos1 ("Portal 1 Position", Vector) = (0,0,0)
		_PortalRot1 ("Portal 1 Rotation", Vector) = (0,0,0)
        _PortalTime1 ("Portal 1 Time", Float) = 0.0
        
		_PortalPos2 ("Portal 2 Position", Vector) = (0,0,0)
		_PortalRot2 ("Portal 2 Rotation", Vector) = (0,0,0)
        _PortalTime2 ("Portal 1 Time", Float) = 0.0
        
		_StencilMask("Stencil Mask", Int) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Int) = 3 //Default Always=8, Equal = 3
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Stencil{
			Ref[_StencilMask]
			Comp [_StencilComp]
			Pass keep
			Fail keep
		}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
			float3 worldNormal;
            float4 color : COLOR;
		};

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

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
        
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
        }
        
        float3x3 matrixRotZ(float theta) {
            float3x3 out_matrix = {
                cos(theta), -sin(theta), 0,
                sin(theta), cos(theta), 0,
                0, 0, 1
            };
            return out_matrix;
        }
        
        bool discardOval(float3 pos, float3 center, float2 rot, float radius) {
            pos = pos - center;
            pos = mul(matrixRotY(rot.y), pos);
            pos = mul(matrixRotX(rot.x), pos);
            pos = pos * float3(1.0, 0.5, 5.0);
            if (length(pos) < radius)
                return true;
            return false;
        }

		void surf (Input IN, inout SurfaceOutputStandard o) {

			if (dot(IN.worldPos.xyz - (_PlanePos.xyz + _PlaneDir.xyz/10.0), _PlaneDir.xyz) > 0)
				discard;
            
            if (discardOval(IN.worldPos.xyz, _PortalPos1, _PortalRot1.xy, smoothstep(0.0, 0.5, _Time.y - _PortalTime1) * 0.5))
                discard;
            
            if (discardOval(IN.worldPos.xyz, _PortalPos2, _PortalRot2.xy, smoothstep(0.0, 0.5, _Time.y - _PortalTime2) * 0.5))
                discard;

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
