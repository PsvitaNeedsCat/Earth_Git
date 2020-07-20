Shader "Earth/Summon Tile"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Base Color", Color) = (1, 1, 1, 1)
		_LitColor("Selected Colour", Color) = (1, 1, 1, 1)
		_Blend("Blend", Range(0.0, 0.3)) = 0.0
		_SymbolTex("Symbol Texture", 2D) = "white" {}
		_Cutoff("Symbol Cutoff", Range(0.0, 1.0)) = 0.0
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
	fixed3 _Color;
	fixed3 _LitColor;

	float _Blend;

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed3 c1 = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
		fixed3 c2 = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color + _LitColor;

		o.Albedo = lerp(c1, c2, (_SinTime.w * 0.15) + 0.15);
	}
		ENDCG

			CGPROGRAM
			#pragma surface surf Flat alphatest:_Cutoff

			half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
		{
			return half4(o.Albedo, 1);
		}

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _SymbolTex;

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_SymbolTex, IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_SymbolTex, IN.uv_MainTex).a;
		}
		ENDCG
    }

	Fallback "Unlit/Color"
}
