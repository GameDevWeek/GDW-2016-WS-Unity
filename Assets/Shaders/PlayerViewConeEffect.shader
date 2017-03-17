
Shader "PlayerViewConeEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_VisibilityTexture ("VisibilityTexture (R)", 2D) = "white" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _VisibilityTexture;

		uniform half4 _MainTex_TexelSize;

		half4 _MainTex_ST;
		
		uniform half4 _Parameter;
		uniform half4 _InvisibleTint;
		uniform half4 _FullscreenTint;

		struct v2f_simple 
		{
			float4 pos : SV_POSITION; 
			half2 uv : TEXCOORD0;

        #if UNITY_UV_STARTS_AT_TOP
				half2 uv2 : TEXCOORD1;
		#endif
		};	
		
		v2f_simple vert ( appdata_img v )
		{
			v2f_simple o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
        	
        #if UNITY_UV_STARTS_AT_TOP
        	o.uv2 = o.uv;
        	if (_MainTex_TexelSize.y < 0.0)
        		o.uv.y = 1.0 - o.uv.y;
        #endif
        	        	
			return o; 
		}

		fixed4 changeSaturation(fixed4 c, float visibility) {
			float sat = lerp(_Parameter.x, _Parameter.y, visibility);
			float lum = lerp(_Parameter.z, _Parameter.w, visibility);

			fixed3 f = fixed3(0.299,0.587,0.114);
			float p = sqrt(c.r*c.r*f.r + c.g*c.g*f.g + c.b*c.b*f.b);
			c.rgb = p*lum + (c-p)*sat;

			c = lerp(_InvisibleTint*c, c, visibility);

			return c;
		}

		fixed4 frag ( v2f_simple i ) : SV_Target
		{	
        	#if UNITY_UV_STARTS_AT_TOP
			
			fixed4 color = tex2D(_MainTex, i.uv2);
			float visibility = tex2D(_VisibilityTexture, i.uv2).r;
			
			#else

			fixed4 color = tex2D(_MainTex, i.uv);
			float visibility = tex2D(_VisibilityTexture, i.uv).r;
						
			#endif

			color = changeSaturation(color, visibility);
			color.rgb = lerp(color.rgb, color.rgb*0.2 + color.rgb*_FullscreenTint.rgb + _FullscreenTint.rgb*0.5, _FullscreenTint.a);
			return color;
		} 
					
	ENDCG
	
	SubShader {
	  ZTest Off Cull Off ZWrite Off Blend Off
	  
	// 0
	Pass {
	
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		
		ENDCG
		 
		}
	}	

	FallBack Off
}
