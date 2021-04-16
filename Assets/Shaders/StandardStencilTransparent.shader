Shader "CustomStandard/StandardStencilTransparent" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_BumpMap("Normal Map", 2D) = "bump" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[MaterialToggle] _DoEmission("Emission", Float) = 0
		[HDR] _Emission("Color", Color) = (1, 1, 1, 1)
		_StencilMask("Stencil Mask", Int) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comp", Int) = 8 //Default Always=8, Equal=3
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		Stencil{
		Ref[_StencilMask]
		Comp[_StencilComp]
		Pass keep
		Fail keep
	}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows alpha:blend 

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _BumpMap;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float _DoEmission;
	half3 _Emission;

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
		if (_DoEmission) {
			o.Emission = _Emission;
		}
	}
	ENDCG
	}
		FallBack "Diffuse"
}
