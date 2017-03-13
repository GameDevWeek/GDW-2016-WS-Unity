Shader "PixelShader"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Pixels ("Size of Pixels", Range (1, 100)) = 1
		_ScreenRes ("Screenresolution", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
 
			#include "UnityCG.cginc"
 
			uniform sampler2D _MainTex;
			uniform int _Pixels;
			uniform float4 _ScreenRes;
 
			float4 frag(v2f_img i) : COLOR {
				float2 anzTiles = _ScreenRes.xy / _Pixels;
				
				float2 uv = float2( int2(i.uv * anzTiles)) / anzTiles;
				float4 result = tex2D(_MainTex, uv);
				return result;
			}
			ENDCG
		}
	}
}