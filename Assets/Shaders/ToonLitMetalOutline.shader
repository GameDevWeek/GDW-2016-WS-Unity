Shader "Toon/Lit Metal Outline" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_NoiseTex("Noise", 2D) = "white" {}
		_TextureIntensity("Texture Intensity", Float) = 1.0
		_FogSpeed("Fog speed", Float) = 1.0
		_FogIntensity("Fog intensity", Float) = 1.0
	}

		SubShader{
		Tags{ "RenderType" = "Opague" }
		LOD 200

			CGPROGRAM
#pragma surface surf ToonRamp

			float _FogSpeed;
		sampler2D _NoiseTex;

		float matColor(float2 vert) {
			return tex2D(_NoiseTex, vert*0.003 + float2(1, 0.2)*_Time.y*_FogSpeed).r * tex2D(_NoiseTex, vert*0.0001 + float2(0.2, -0.2)*_Time.y*_FogSpeed).g;
		}



		sampler2D _Ramp;

		inline float ramp(float i) {
			float d = i*0.5 + 0.5;
			return tex2D(_Ramp, float2(d, d)).r;
		}

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
#endif

			half3 h = normalize(lightDir + viewDir);

			float diff = max(0.3, saturate(dot(s.Normal, lightDir)) * 0.5);
			float spec = pow(saturate(dot(s.Normal, h)), 8)*1.2;

			half4 c;
			c.rgb = ramp(diff + spec) * s.Albedo * _LightColor0.rgb * (atten * 2);
			c.a = 1;
			return c;
		}


		sampler2D _MainTex;
		sampler2D _NormalMap;
		float4 _Color;
		float _TextureIntensity;
		float _FogIntensity;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));

			half4 c = lerp(half4(1, 1, 1, 1), tex2D(_MainTex, IN.uv_MainTex), _TextureIntensity) * _Color;

			half3 blendWeights = pow(abs(WorldNormalVector(IN, o.Normal)), 4.0);
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

			if (_FogIntensity>0.0) {
				float fog = matColor(IN.worldPos.xy * 100) * blendWeights.z
					+ matColor(IN.worldPos.zy * 100) * blendWeights.x
					+ matColor(IN.worldPos.xz * 100) * blendWeights.y;

				c.rgb *= lerp(1.0, 0.4 + fog, _FogIntensity);
			}

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG


	CGINCLUDE
#include "UnityCG.cginc"

		struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		UNITY_FOG_COORDS(0)
			fixed4 color : COLOR;
	};

	uniform float _Outline;
	uniform float4 _OutlineColor;

	v2f vert(appdata v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

		float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
		float2 offset = TransformViewToProjection(norm.xy);

		float outline = _Outline + (sin(_Time[3]) + 0.5) * _Outline * 0.4;
#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
		o.pos.xy += offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.pos.z) * outline;
#else
		o.pos.xy += offset * o.pos.z * outline;
#endif

		o.color = _OutlineColor;
		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}
	ENDCG
	Pass{
		Name "OUTLINE"
		Cull Front
		ZWrite On
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		fixed4 frag(v2f i) : SV_Target
	{
		return i.color;
	}
		ENDCG
	}
	}
	Fallback "Diffuse"
}
