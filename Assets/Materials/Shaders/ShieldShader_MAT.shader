Shader "Earth/Shield"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ShieldColor("Shield Colour", Color) = (1, 1, 1, 1)
		_ShieldScroll ("Scroll Speed", Float) = 1.0
		_ShieldTex("Shield Texture", 2D) = "white"{}
		_ShieldEmissive("Shield Emssive Color", Color) = (1, 1, 1, 1)
		_Cutoff("Alpha Cutoff", Range(0.0, 1.1)) = 0.5
		_FresnelColor("Fresnel Colour", Color) = (1, 1, 1, 1)
		_FresnelStrength("Fresnel Strength", Range(0.0, 5.0)) = 1.0
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
				float3 worldNormal;
				float3 viewDir;
				INTERNAL_DATA
			};

            sampler2D _MainTex;

			float3 _FresnelColor;
			float _FresnelStrength;

			void surf(Input IN, inout SurfaceOutput o)
			{
				float fresnel = dot(IN.worldNormal, IN.viewDir);
				fresnel = saturate(1 - fresnel);
				fresnel = pow(fresnel, _FresnelStrength);
				float3 fresnelColor = fresnel * _FresnelColor;
				

				o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
				o.Emission = fresnelColor;
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

		Fallback "Unlit/Texture"
}
