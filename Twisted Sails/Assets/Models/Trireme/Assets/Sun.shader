Shader "Custom/TriremeSun" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Normal1("Bumpmap", 2D) = "bump" {}
		_Normal2("Bumpmap", 2D) = "bump" {}
		_NormalBlend("Blend", Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard noshadow nodirlightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Normal1;
		sampler2D _Normal2;

		struct Input {
			float2 uv_Normal1;
			float2 uv_Normal2;
		};

		half _Glossiness;
		half _NormalBlend;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			o.Albedo = _Color;
			o.Emission = _Color * .5f;
			// Metallic and smoothness come from slider variables
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
			o.Normal = UnpackNormal(tex2D(_Normal1, IN.uv_Normal1) * _NormalBlend + tex2D(_Normal2, IN.uv_Normal2) * (1-_NormalBlend));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
