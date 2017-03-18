Shader "Toon/LitMetal" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
		_NoiseTex ("Noise", 2D) = "white" {}
		_TextureIntensity ("Texture Intensity", Float) = 1.0
		_FogSpeed ("Fog speed", Float) = 1.0
		_FogIntensity ("Fog intensity", Float) = 1.0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
CGPROGRAM
#pragma surface surf ToonRamp

		float _FogSpeed;
		sampler2D _NoiseTex;

		float matColor (float2 vert) {
			return tex2D(_NoiseTex, vert*0.003 + float2(1,0.2)*_Time.y*_FogSpeed).r * tex2D(_NoiseTex, vert*0.0001 + float2(0.2,-0.2)*_Time.y*_FogSpeed).g;
		}



		sampler2D _Ramp;

		inline float ramp(float i) {
			float d = i*0.5 + 0.5;
			return tex2D (_Ramp, float2(d,d)).r;
		}

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
			
			half3 h = normalize(lightDir + viewDir);

			float diff = max(0.3, saturate(dot (s.Normal, lightDir)) * 0.5);
			float spec = pow(saturate(dot (s.Normal, h)), 8)*1.2;
			
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

		void surf (Input IN, inout SurfaceOutput o) {
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));

			half4 c = lerp(half4(1,1,1,1), tex2D(_MainTex, IN.uv_MainTex), _TextureIntensity) * _Color;

			half3 blendWeights = pow (abs(WorldNormalVector (IN, o.Normal)), 4.0);
			blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);

			if(_FogIntensity>0.0) {
				float fog = matColor(IN.worldPos.xy*100) * blendWeights.z
				          + matColor(IN.worldPos.zy*100) * blendWeights.x
				          + matColor(IN.worldPos.xz*100) * blendWeights.y;

				c.rgb *= lerp(1.0, 0.4 + fog, _FogIntensity);
			}

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

	ENDCG

	} 

	Fallback "Diffuse"
}
