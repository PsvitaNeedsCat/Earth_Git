Shader "Earth/Emissive"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_EmissiveColor ("Emissive Colour", Color) = (1, 1, 1, 1)
		_EmissionTex("Emission Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		CGPROGRAM
		#pragma surface surf Flat

		half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
		{
			return half4(o.Albedo, 1);
		}

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _EmissionTex;

		fixed3 _EmissiveColor;

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Emission = tex2D(_EmissionTex, IN.uv_MainTex).rgb * _EmissiveColor;
		}


		ENDCG
    }
		Fallback "Unlit/Color"
}
