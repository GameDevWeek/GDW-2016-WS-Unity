Shader "Custom/NewSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;


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
			for (int i = 0; i < 15; ++i) {
				v += a * noise(_st);
				_st =  _st * 2.0 + shift;
				a *= 0.5;
			}
			return v;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 st = IN.uv_MainTex / float2(1024,1024)*100.;
			// st += st * abs(sin(_Time.y*0.1)*3.0);
			float3 color = float3(0.0, 0.0, 0.0);

			float2 q = float2(0., 0.);
			q.x = fbm(st + 0.00*_Time.y);
			q.y = fbm(st + float2(1.0, 1.0));

			float2 r = float2(0., 0.);
			r.x = fbm(st + 1.0*q + float2(1.7, 9.2) + 0.15*_Time.y);
			r.y = fbm(st + 1.0*q + float2(8.3, 2.8) + 0.126*_Time.y);

			float f = fbm(st + r);

			color = lerp(float3(0.101961, 0.619608, 0.666667),
				float3(0.666667, 0.666667, 0.498039),
				clamp((f*f)*4.0, 0.0, 1.0));

			color = lerp(color,
				float3(0, 0, 0.164706),
				clamp(length(q), 0.0, 1.0));

			color = lerp(color,
				float3(0.666667, 1, 1),
				clamp(length(r.x), 0.0, 1.0));

			

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = float4((f*f*f + .6*f*f + .5*f)*color, 1.);
			//o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
