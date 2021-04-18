Shader "Unlit/PlanarShadow"
{
    Properties
    {
        _VerticalOffset ("Vertical Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float _VerticalOffset;

            v2f vert (appdata v)
            {
                v2f o;
                
                //float4 worldLightDirection = -normalize(_WorldSpaceLightPos0);
                
                float3 lightDir = normalize(
                        unity_LightPosition[0] - mul(UNITY_MATRIX_MV, v.vertex));
                lightDir = mul(UNITY_MATRIX_IT_MV, lightDir);
                
                float4 planeNormal = float4(0,1,0,1);
                float planeNormalDotWorldVertex = dot(planeNormal, mul(unity_ObjectToWorld, v.vertex + float3(0,_VerticalOffset,0)));
                float planeNormalDotLightDir = dot(planeNormal, lightDir);
                
                float3 worldVertexToPlaneVector = lightDir * (planeNormalDotWorldVertex / (-planeNormalDotLightDir));
                
                o.vertex = UnityObjectToClipPos(v.vertex + mul(unity_WorldToObject, worldVertexToPlaneVector));
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = fixed4(0,0,0,1);
                return col;
            }
            ENDCG
        }
    }
}
