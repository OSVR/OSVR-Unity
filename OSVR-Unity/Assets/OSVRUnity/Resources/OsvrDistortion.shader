/// OSVR-Unity Connection
///
/// http://sensics.com/osvr
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///		 http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>
/// <summary>
/// Long ago, based on Unity's (Pro Only) Image Effect: Fisheye.shader
/// Author: Greg Aring
/// Email: greg@sensics.com
/// </summary>

/// Syntax reference: see http://docs.unity3d.com/Manual/SL-Shader.html

Shader "Osvr/OsvrDistortion" {
Properties
{
	_K1_Red ("K1 Red", Range (0.00,1.00)) = 0.0 // sliders
	_K1_Green ("K1 Green", Range (0.00,1.00)) = 0.0
	_K1_Blue ("K1 Blue", Range (0.00,1.00)) = 0.0
	_Center ("Center of Projection", Vector) = (.5,.5,0,0)
	_MainTex ("Base (RGB)", 2D) = "" {}
}
	CGINCLUDE
	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	float _K1_Red;
	float _K1_Green;
	float _K1_Blue;
	float4 _Center;
	sampler2D _MainTex;

	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	float2 Distort(float2 p, float k1)
	{
    /// @todo would pow improve performance here? (by using SFU if available?)
		float r2 = p.x * p.x + p.y * p.y;

		float newRadius = (1 + k1*r2);
		p.x = p.x * newRadius;
		p.y = p.y * newRadius;

		return p;
	}

	half4 frag(v2f i) : SV_Target
	{
		float2 uv_red, uv_green, uv_blue;
		float4 color_red, color_green, color_blue;
		float2 sectorOrigin;
		float4 color;

		sectorOrigin = _Center.xy;

		uv_red = Distort(i.uv-sectorOrigin, _K1_Red) + sectorOrigin;
		uv_green = Distort(i.uv-sectorOrigin, _K1_Green) + sectorOrigin;
		uv_blue = Distort(i.uv-sectorOrigin, _K1_Blue) + sectorOrigin;

		color_red = tex2D(_MainTex, uv_red);
		color_green = tex2D(_MainTex, uv_green);
		color_blue = tex2D(_MainTex, uv_blue);

		if( ((uv_red.x > 0) && (uv_red.x < 1) && (uv_red.y > 0) && (uv_red.y < 1)))
		{
			color = float4(color_red.x, color_green.y, color_blue.z, 1.0);
		}
		else
		{
			color = float4(0,0,0,1);
		}
		return color;
	}
	ENDCG

	Subshader {
	 Pass {
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

	}
}
