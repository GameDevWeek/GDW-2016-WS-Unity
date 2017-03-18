Shader "Toon/Lit Fluid"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100 

		Cull Back

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf ToonRamp
		#include "noiseSimplex.cginc"

		sampler2D _Ramp; 

		float fbm(float3 _st) {
			float v = 0.0;
			float a = 0.5;
			float3 shift = float3(100.0, 100.0, 100.0);
			// Rotate to reduce axial bias
			//float2x2 rot = float2x2(cos(0.5), sin(0.5),
			//	-sin(0.5), cos(0.50));
			for (int i = 0; i < 6; ++i) {
				v += a * (snoise(_st) * 0.5 + 0.5);
				_st = _st * 2.0 + shift;
				a *= 0.5;
			}
			return v;
		}

		float3 fluidColor(float3 worldPos)
		{
			float3 st = worldPos / float3(1024, 1024, 1024) * 200;
			// st += st * abs(sin(_Time.y*0.1)*3.0);
			float3 color = float3(0.0, 0.0, 0.0);

			float3 q = float3(0., 0., 0.);
			q.x = fbm(st + 0.00*_Time.y);
			q.y = fbm(st + float3(1.0, 1.0, 1.0));

			float3 r = float3(0., 0., 0.);
			r.x = fbm(st + 1.0*q + float3(1.7, 9.2, 0) + 0.15*_Time.y);
			r.y = fbm(st + 1.0*q + float3(8.3, 2.8, 0) + 0.126*_Time.y);

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

			float3 col = float3((f*f*f + .6*f*f + .5*f)*color);

			return col;
		}

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif

			half d = dot(s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = tex2D(_Ramp, float2(d, d)).rgb;

			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
			return c;
		}

		sampler2D _MainTex;
		sampler2D _NormalMap;
		float4 _Color;

		struct Input {
			float3 worldPos;
			float2 uv_MainTex : TEXCOORD0;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = fluidColor(IN.worldPos) * c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
		}
		ENDCG
	}
	Fallback "Diffuse"

}
