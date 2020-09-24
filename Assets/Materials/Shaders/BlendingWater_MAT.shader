Shader "Earth/Blending Water"
{
	Properties
	{
		_Color("Base Colour", Color) = (1, 1, 1, 1)
		_MainTex("Texture 1", 2D) = "white" {}
		_Dist("Distortion", Float) = 0.1
		_Blend("Blend Factor", Range(0, 1)) = 0.0
		_BlendTex("Blend Texture", 2D) = "white"{}
		[HDR] _EmissionColor("Emissive Color", Color) = (0, 0, 0, 1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

				CGPROGRAM
				#pragma surface surf Flat vertex:vert

				fixed4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
				{
					return fixed4(o.Albedo, 1);
				}

				struct Input
				{
					float2 st_MainTex;
					float2 uv_BlendTex;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _MainTex2;
				float4 _MainTex2_ST;
				float _Dist;

				void vert(inout appdata_full v, out Input o)
				{
					UNITY_INITIALIZE_OUTPUT(Input, o);

					o.st_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

					o.st_MainTex.x += sin((o.st_MainTex.x + o.st_MainTex.y) + _Time.g) * _Dist;
					o.st_MainTex.y += cos((o.st_MainTex.x - o.st_MainTex.y) + _Time.g) * _Dist;
				}

				float _Blend;
				sampler2D _BlendTex;

				fixed4 _EmissionColor;

				float2 UVDistort(float2 uv, float time)
				{
					return uv + time;
				}

				float2 moveUV(float2 uv, float time)
				{
					return float2(uv.x, uv.y + time);
				}

				void surf(Input IN, inout SurfaceOutput o)
				{
					float2 uv = moveUV(IN.st_MainTex, _Time.y);

					float4 c1 = tex2D(_MainTex, IN.st_MainTex);
					float4 c2 = tex2D(_MainTex, uv);
					float4 b = tex2D(_BlendTex, IN.uv_BlendTex);

					o.Albedo = lerp(c1, c2, _Blend * b).rgb;
					o.Emission = _EmissionColor;
				}
				ENDCG
		}
			Fallback "Diffuse"
}
