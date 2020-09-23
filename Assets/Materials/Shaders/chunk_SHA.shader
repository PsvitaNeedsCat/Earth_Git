Shader "Earth/Chunk"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_CrackTex("Crack Texture", 2D) = "white" {}
		_Cutoff("Crack Alpha Cutoff", Float) = 0.5
		[Toggle] _Crack("Cracked", Float) = 0
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
		sampler2D _CrackTex;

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
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

		sampler2D _CrackTex;
		float _Crack;

		void surf(Input IN, inout SurfaceOutput o)
		{
			if (_Crack)
			{
				o.Albedo = tex2D(_CrackTex, IN.uv_MainTex).rgb;
				o.Alpha = tex2D(_CrackTex, IN.uv_MainTex).a;
			}
		}

		ENDCG
    }
		Fallback "Unlit/Texture"
}
