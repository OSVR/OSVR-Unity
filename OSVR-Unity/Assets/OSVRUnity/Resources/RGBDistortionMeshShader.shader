// Unlit shader with 3 sets of UVs
// - no lighting
// - no lightmap support
// - no per-material color

Shader "OSVR/RGBDistortionMeshShader" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoordR : TEXCOORD0;
				float2 texcoordG : TEXCOORD1;
				float2 texcoordB : TEXCOORD2;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoordR : TEXCOORD0;
				half2 texcoordG : TEXCOORD1;
				half2 texcoordB : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoordR = TRANSFORM_TEX(v.texcoordR, _MainTex);
				o.texcoordG = TRANSFORM_TEX(v.texcoordG, _MainTex);
				o.texcoordB = TRANSFORM_TEX(v.texcoordB, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 r = tex2D(_MainTex, i.texcoordR);
	            float4 g = tex2D(_MainTex, i.texcoordG);
	            float4 b = tex2D(_MainTex, i.texcoordB);
				return float4(r.r, g.g, b.b, 1.0f);
			}
		ENDCG
	}
}

}
