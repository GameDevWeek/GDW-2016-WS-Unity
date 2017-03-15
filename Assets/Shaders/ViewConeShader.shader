Shader "Rendering/ViewCone" {

	Properties
	{
		_Color("Main Color (A=Opacity)", Color) = (1,1,1,1)
		_OutlineTex("Outline Texture", 2D) = "black" {}
		_MinAlpha("Minimum Alpha", Float) = 0.2
		_MaxAlpha("Maximum Alpha", Float) = 0.8
		_LineThickness("Line Thickness", Float) = 0.5
		_LineSpeed("Line Movement Speed", Float) = 0.5
	}

	SubShader
	{
		//Blend One One
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 world_vertex : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				float depth : TEXCOORD2;
				float2 uv : TEXCOORD3;
			};

			sampler2D _OutlineTex;
			float4 _Color;
			float _MinAlpha;
			float _MaxAlpha;
			float _LineThickness;
			float _LineSpeed;
			sampler2D _CameraDepthNormalsTexture;

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
				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;
				o.screenuv.y = 1 - o.screenuv.y;
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;
				o.uv = v.uv;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				float diff = screenDepth - i.depth;
				float intersect = 0;

				if (diff > 0)
					intersect = 1 - smoothstep(0, _ProjectionParams.w * 0.75, diff);
				fixed4 intersectColor = fixed4(lerp(_Color.rgb, fixed3(1, 1, 1), pow(intersect, 4)), 1);

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

				float3 outline = tex2D(_OutlineTex, i.uv).rgb;

				fixed4 col = _Color /*+ intersectColor * intersect*/ + fixed4(outline,0);
				col.a *= a + outline.r;
				return col;
			}
			ENDCG
		}
	}

}