Shader "Earth/Fire Grub"
{
    Properties
    {
        _MainTex ("Cold Texture", 2D) = "white" {}
		_MainEmission ("Cold Emission", Color) = (0, 0, 0, 1)
		_MainEmissionTex ("Cold Emission Texture", 2D) = "white" {}
		_HotTex ("Hot Texture", 2D) = "white" {}
		_HotEmission ("Cold Emission", Color) = (1, 1, 1, 1)
		_HotEmissionTex("Hot Emission Texture", 2D) = "white" {}
		_Blend ("Blend", Range(0.0, 1.0)) = 0.0
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
		sampler2D _HotTex;
		fixed _Blend;

		sampler2D _MainEmissionTex;
		sampler2D _HotEmissionTex;

		fixed3 _MainEmission;
		fixed3 _HotEmission;

		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 c = lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_HotTex, IN.uv_MainTex), _Blend).rgb;

			float3 em1 = tex2D(_MainEmissionTex, IN.uv_MainTex).rgb * _MainEmission;
			float3 em2 = tex2D(_HotEmissionTex, IN.uv_MainTex).rgb * _HotEmission;

			o.Albedo = c;
			o.Emission = lerp(em1, em2, _Blend);
		}

		ENDCG
    }
		Fallback "Unlit/Color"
}
