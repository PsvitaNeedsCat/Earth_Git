Shader "Earth/Mirage Boss"
{
	Properties
	{
		_MainTex("Main Texure", 2D) = "white"{}
		_Color("Start Colour", Color) = (1, 1, 1, 1)
		[HDR]_Color2("End Colour", Color) = (1, 1, 1, 1)
		_Noise("Noise Blend", 2D) = "white" {}
		_NoisePow("Noise Softness", Float) = 1
		_Cutoff("dissolve Cutoff", Range(0.0, 1.0)) = 0.0
			[Space]
		_ShieldColor("Shield Colour", Color) = (1, 1, 1, 1)
		_ShieldScroll("Scroll Speed", Float) = 1.0
		_ShieldTex("Shield Texture", 2D) = "white"{}
		_ShieldEmissive("Shield Emssive Color", Color) = (1, 1, 1, 1)
		_ShieldCutoff ("Shield Cutoff", Range(0.0, 1.0)) = 0.5
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

		//Transparency
			
		Lighting Off
		ZWrite Off
		Cull Off
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

		ZWrite On
		Cull Off

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

		CGPROGRAM

		#pragma surface surf Flat alphatest:_ShieldCutoff

			half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
		{
			return half4(o.Albedo, 1);
		}

		struct Input
		{
			float2 uv_ShieldTex;
		};

		sampler2D _ShieldTex;
		fixed3 _ShieldColor;
		fixed3 _ShieldEmissive;

		fixed _ShieldScroll;

		float2 moveUV(float2 uv, float time)
		{
			return float2(uv.x, uv.y + (_ShieldScroll * time));
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float2 uv = moveUV(IN.uv_ShieldTex, _Time.y);
			fixed4 col = tex2D(_ShieldTex, uv);

			o.Albedo = col.rgb * _ShieldColor.rgb;
			o.Alpha = col.a;
			o.Emission = col.rgb * _ShieldEmissive;
		}
			
		ENDCG

		}
			FallBack "Unlit/Transparent"
}