Shader "Earth/Lava Tile"
{
    Properties
    {
		[Header(Main Values)]
		_Color("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
		_Emission("Emissive Color", Color) = (1, 1, 1, 1)
		[Space]
		[Header(Border)]
		_BorderTex("Border Texture", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
		[Space]
		[Header(Distortion)]
		_Dist ("Distortion Factor", Float) = 0.0
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

            CGPROGRAM
            #pragma surface surf Flat //vertex:vert

			half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
			{
				return half4(o.Albedo, 1);
			}
			
			sampler2D _MainTex;
			//float4 _MainTex_ST;

			struct Input
			{
				//float2 st_MainTex;
				float2 uv_MainTex;
			};

			//float _Dist;

			/*void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);

				o.st_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.st_MainTex.x += sin((o.st_MainTex.x + o.st_MainTex.y) + _Time.g) * _Dist;
				o.st_MainTex.y += cos((o.st_MainTex.x - o.st_MainTex.y) + _Time.g) * _Dist;
			}*/

			fixed4 _Color;
			fixed4 _Emission;

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rbg * _Color.rgb;
				//o.Emission = _Emission.rgb;
			}

			ENDCG

			CGPROGRAM
			#pragma surface surf Flat alphatest:_Cutoff
			#pragma target 2.0

			half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
			{
				return half4(o.Albedo, 1);
			}

			sampler2D _BorderTex;

			struct Input
			{
				float2 uv_BorderTex;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = tex2D(_BorderTex, IN.uv_BorderTex).rgb;
				o.Alpha = tex2D(_BorderTex, IN.uv_BorderTex).a;
			}

			ENDCG
    }

	Fallback "Unlit/Texture"
}
