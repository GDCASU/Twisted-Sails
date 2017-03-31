Shader "Custom/Waterfall" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_LatticePatternTex("LatticePattern (BW)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _LatticePatternTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv_LatticePatternTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb * (.5 * tex2D(_LatticePatternTex, IN.uv_LatticePatternTex).r + 1);
	}
	ENDCG
	}
		FallBack "Diffuse"
}
