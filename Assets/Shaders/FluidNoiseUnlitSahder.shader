Shader "Toon/Lit Simple Fluid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
		
		Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// make fog work

			
			#include "UnityCG.cginc"

#include "UnityLightingCommon.cginc" // for _LightColor0

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

	float2 random(float2 _st) {
		return frac(sin(dot(_st.xy,
			float2(12.9898, 78.233)))*
			43758.5453123);
	}

	float noise(in float2 _st) {
		float2 i = floor(_st);
		float2 f = frac(_st);

		// Four corners in 2D of a tile
		float a = random(i);
		float b = random(i + float2(1.0, 0.0));
		float c = random(i + float2(0.0, 1.0));
		float d = random(i + float2(1.0, 1.0));

		float2 u = f * f * (3.0 - 2.0 * f);

		return lerp(a, b, u.x) +
			(c - a)* u.y * (1.0 - u.x) +
			(d - b) * u.x * u.y;
	}

	float fbm(float2 _st) {
		float v = 0.0;
		float a = 0.5;
		float2 shift = float2(100.0, 100.0);
		// Rotate to reduce axial bias
		float2x2 rot = float2x2(cos(0.5), sin(0.5),
			-sin(0.5), cos(0.50));
		for (int i = 0; i < 6; ++i) {
			v += a * noise(_st);
			_st = _st * 2.0 + shift;
			a *= 0.5;
		}
		return v;
	}

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0; // diffuse lighting color
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _NormalMap;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diff = nl * _LightColor0;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				float2 st = i.vertex.xy / float2(1024, 1024) * 20;
				// st += st * abs(sin(_Time.y*0.1)*3.0);
				float3 color = float3(0.0, 0.0, 0.0);

				float2 q = float2(0., 0.);
				q.x = fbm(st + 0.00*_Time.y);
				q.y = fbm(st + float2(1.0, 1.0));

				float2 r = float2(0., 0.);
				r.x = fbm(st + 1.0*q + float2(1.7, 9.2) + 0.15*_Time.y);
				r.y = fbm(st + 1.0*q + float2(8.3, 2.8) + 0.126*_Time.y);

				float f = fbm(st + r);

				color = lerp(float3(1, 0.619608, 0.666667),
					float3(0.666667, 0.666667, 1),
					clamp((f*f)*4.0, 0.0, 1.0));

				color = lerp(color,
					float3(0, 1, 0.164706),
					clamp(length(q), 0.0, 1.0));

				color = lerp(color,
					float3(0.666667, 1, 1),
					clamp(length(r.x), 0.0, 1.0));

				fixed4 col = float4((f*f*f + .6*f*f + .5*f)*color, 1.);
/*				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);*/
				
				return col;
			}
			ENDCG
		}


	}

}
