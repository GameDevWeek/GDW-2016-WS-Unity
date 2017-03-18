Shader "Toon/Lit Outline Working" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		//Blend SrcAlpha OneMinusSrcAlpha
		//Cull Off

		LOD 200

		Pass{
		ZWrite On
		ColorMask 0
	}

		//ZTest Greater ZWrite On

		CGPROGRAM
#pragma surface surf ToonRamp alpha

		sampler2D _Ramp;

	// custom lighting function that uses a texture ramp based
	// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
	inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
	{
#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(lightDir);
#endif

		half d = dot(s.Normal, lightDir)*0.5 + 0.5;
		half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
		c.a = s.Alpha;
		return c;
	}


	sampler2D _MainTex;
	sampler2D _NormalMap;
	float4 _Color;

	struct Input {
		float2 uv_MainTex : TEXCOORD0;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
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
