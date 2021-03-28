Shader "Custom/PlaneCut" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_PlanePos ("Plane Position", Vector) = (0,0,0)
		_PlaneDir ("Plane Direction (Euler)", Vector) = (0,0,0)
		//_PortalPos1 ("Portal 1 Position", Vector) = (0,0,0)
		//_PortalScale1 ("Portal 1 Scale", Vector) = (0,0,0)
		//_PortalRot1("Portal 1 Rotation", Vector) = (0,0,0)
		//_PortalPos2("Portal 2 Position", Vector) = (0,0,0)
		//_PortalScale2("Portal 2 Scale", Vector) = (0,0,0)
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
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
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
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float3 _PlanePos;
		float3 _PlaneDir;

		float3 _PortalPos1;
		float3 _PortalPos2;

		float3 _PortalScale1;
		float3 _PortalScale2;
		
		float4 _PortalRot1;
		float4 _PortalRot2;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void getrotmatrix(in float4 q, out float3x3 R) {
			//Finds rotation matrice for a normalized vector (q.xyz) and an angle (q.w)
			//WTF THIS DOESNT WORK
			q.w = q.w * 0.0174533;
			float3x3 RO = { cos(q.w) + (pow(q.x,2) * (1 - cos(q.w))), q.x*q.y*(1 - cos(q.w)) - q.z*sin(q.w), q.x*q.z*(1 - cos(q.w)) + q.y*sin(q.w),
				q.y*q.x*(1 - cos(q.w)) + q.z*sin(q.w), cos(q.w) + pow(q.y, 2)*(1 - cos(q.w)), q.y*q.z*(1 - cos(q.w)) - q.x*sin(q.w),
				q.z*q.x*(1 - cos(q.w)) - q.y*sin(q.w), q.z*q.y*(1 - cos(q.w)) + q.x*sin(q.w), cos(q.w) + pow(q.z,2)*(1 - cos(q.w)) };
			R = RO;

			/*float cx = cos(q.x * 0.0174533);
			float sx = sin(q.x * 0.0174533);
			float cy = cos(q.y * 0.0174533);
			float sy = sin(q.y * 0.0174533);
			float cz = cos(q.z * 0.0174533);
			float sz = sin(q.z * 0.0174533);

			R._m00 = cx*cz - sx*sy*sz;
			R._m01 = -1 * cy * sx;
			R._m02 = cx * sz + cz * sx * sy;
			R._m10 = cz * sx + cx * sy * sz;
			R._m11 = cx * cy;
			R._m12 = sx * sz - cx * cz * sy;
			R._m20 = -1 * cy * sz;
			R._m21 = sy;
			R._m22 = cy * cz;*/
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			if (dot(IN.worldPos.xyz - _PlanePos, _PlaneDir.xyz) > 0)
				discard;

			/*if (length(float3(IN.worldPos.x / _PortalScale1.x, IN.worldPos.y / _PortalScale1.y, IN.worldPos.z / _PortalScale1.z) + (-_PortalPos1.xyz/_PortalScale1.xyz)) < 1)
				discard;
			if (length(float3(IN.worldPos.x / _PortalScale2.x, IN.worldPos.y / _PortalScale2.y, IN.worldPos.z / _PortalScale2.z) + (-_PortalPos2.xyz/_PortalScale2.xyz)) < 1)
				discard;*/

			
			//Use angle axis??
			//Swap some axes around to make the equation compatible with unity axes
			/*float4 ER = _PortalRot1;
			float3x3 R;
			getrotmatrix(ER, R);
			float3x1 x = { IN.worldPos.x, IN.worldPos.y, IN.worldPos.z };
			float3x1 off = { _PortalPos1.x, _PortalPos1.y, _PortalPos1.z };
			float3x1 NP = mul(R, x - off);
			float3x3 A = { 1 / pow(_PortalScale1.x, 2), 0, 0,
							0, 1 / pow(_PortalScale1.y, 2), 0,
							0, 0, 1 / pow(_PortalScale1.z, 2) };
			if (mul(mul(mul(transpose(x - off), transpose(R)), A), mul(R, (x - off))) <= 1) {
				discard;
			}*/

			//TEXTURE METHOD??

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
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
