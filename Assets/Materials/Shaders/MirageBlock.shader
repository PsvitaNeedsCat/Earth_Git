Shader "Earth/Mirage Block"
{
	Properties
	{
		_MainTex("Main Texure", 2D) = "white"{}
		_Color("Start Colour", Color) = (1, 1, 1, 1)
		[HDR]_Color2("End Colour", Color) = (1, 1, 1, 1)
		_Noise("Noise Blend", 2D) = "white" {}
		_NoisePow("Noise Softness", Float) = 1
		_Cutoff("dissolve Cutoff", Range(0.0, 1.0)) = 0.0
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

		//Transparency
			
		Lighting Off
		ZWrite Off
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		//Tags {"Queue"="Transparent"}
		Color[_Color2]
		Pass
		{
			Stencil{
				Ref 1	//Ref value
				Comp Greater	//If value greater
				Pass IncrSat	//Increase saturation
			}
		}

		//Solid Overlay

		ZWrite Off
		Cull Back

		CGPROGRAM
		#pragma surface surf Flat alphatest:_Cutoff

			

		half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
		{
			return half4(o.Albedo, 1);
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_Noise;
		};

		sampler2D _MainTex;
		sampler2D _Noise;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{

			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
			o.Alpha = tex2D(_Noise, IN.uv_Noise) * 2;
		}

		ENDCG

		}
			FallBack "Unlit/Transparent"
}