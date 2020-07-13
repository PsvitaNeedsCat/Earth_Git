Shader "Earth/Wave_SHA"
{
    Properties
    {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowTex("Shading Texture", 2D) = "white" {}
        _ScrollX ("Scroll X", Float) = -0.5
		[Space]
		_FoamColor("Foam Color", Color) = (1, 1, 1, 1)
		_FoamTex("Foam Texture", 2D) = "white"{}
		_Cutoff("Foam Cutoff", Range(0.0, 1.0)) = 0.5
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

            sampler2D _MainTex;
            sampler2D _ShadowTex;
            
            struct Input
            {
                float2 uv_MainTex;
                float2 uv_ShadowTex;
            };

            fixed _ScrollX;
			fixed4 _Color;

            void surf(Input IN, inout SurfaceOutput o)
            {
                float uvX = IN.uv_MainTex.x + (_ScrollX * _Time.y);
                float2 newUV = float2(uvX, IN.uv_MainTex.y);
                fixed3 col = tex2D(_MainTex, newUV).rgb;

                fixed3 sha = tex2D(_ShadowTex, IN.uv_ShadowTex).rgb;

                o.Albedo = col * sha * _Color.rgb;
            }

            ENDCG

			CGPROGRAM
			#pragma surface surf Flat alphatest:_Cutoff

			half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
			{
				return half4(o.Albedo, 1);
			}

			sampler2D _FoamTex;

			struct Input
			{
				float2 uv_FoamTex;
			};

			fixed4 _FoamColor;

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 foam = tex2D(_FoamTex, IN.uv_FoamTex);

				o.Albedo = foam.rgb * _FoamColor.rgb;
				o.Alpha = foam.a * _FoamColor.a;
			}

			ENDCG
        }
        FallBack "Particles/Standard Unlit"
}
