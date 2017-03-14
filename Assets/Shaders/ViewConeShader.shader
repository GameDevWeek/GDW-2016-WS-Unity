// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Rendering/ViewCone" {

	Properties
	{
		_Color("Main Color (A=Opacity)", Color) = (1,1,1,1)
		_MinAlpha("Minimum Alpha", Float) = 0.2
		_MaxAlpha("Maximum Alpha", Float) = 0.8
		_LineThickness("Line Thickness", Float) = 0.5
		_LineSpeed("Line Movement Speed", Float) = 0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile_fog

#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 world_vertex : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _Color;
			float _MinAlpha;
			float _MaxAlpha;
			float _LineThickness;
			float _LineSpeed;

			float modulo(float a, float b) {

				a -= floor(a / b)*b;
				if (a < 0) a += b;
				return a;
			}

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.world_vertex = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float pos = modulo((i.world_vertex.x + i.world_vertex.z) * _LineThickness + _Time[1] * _LineSpeed, 1.0);
				float a;
				if (pos < 0.25)
				{
					a = smoothstep(0, 0.25, pos) * (_MaxAlpha - _MinAlpha) + _MinAlpha;
				}
				else if (pos < 0.25)
				{
					a = _MaxAlpha;
				}
				else if (pos < 0.75)
				{
					a = (1 - smoothstep(0.5, 0.75, pos)) * (_MaxAlpha - _MinAlpha) + _MinAlpha;
				}
				else
				{
					a = _MinAlpha;
				}
				fixed4 col = _Color;
				col.a *= a;
				return col;
			}
			ENDCG
		}
	}

}