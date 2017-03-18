Shader "Toon/Lit Alpha ZWrite" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map (RGB)", 2D) = "bump" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		// extra pass that renders to depth buffer only
		Pass{
			ZWrite On
			ColorMask 0
		}

		// paste in forward rendering passes from Transparent/Diffuse
		UsePass "Toon/Lit Alpha/FORWARD"
	}
	Fallback "Toon/Lit Alpha"
}
