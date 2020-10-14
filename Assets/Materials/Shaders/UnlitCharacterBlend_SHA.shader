Shader "Earth/Unlit Character Blend"
{
    Properties
    {
        [Header(MAIN TEXTURE)]
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Texture One", 2D) = "white" {}
		_MainTexTwo("Texture Two", 2D) = "white" {}
		_TextureBlend("Texture Blend", Range(0,1)) = 0.0
        [Space]
        [Header(SILHOUETTE)]
        _SilhouetteColor("Silhouette Color", Color) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "RenderType" = "Opaque"
            }
            LOD 200

            //Depth Occlusion
            ZTest Greater
            ZWrite Off

            CGPROGRAM

            #pragma surface surf Silhouette alpha
            #pragma target 3.0

            half4 LightingSilhouette(SurfaceOutput s, half3 lightDir, half atten)
            {
                return half4(s.Albedo, 1);
            }

            struct Input
            {
                float2 uv;
            };

            float4 _SilhouetteColor;

            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = _SilhouetteColor.rgb;
                o.Alpha = _SilhouetteColor.a;
            }

            ENDCG


            ZWrite On
            ZTest LEqual
            Cull Off

            CGPROGRAM
            #pragma surface surf Flat fullforwardshadows
            #pragma target 3.0

            half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
            {
                return half4(o.Albedo, 1);
            }

            struct Input
            {
                float2 uv_MainTex;
				float2 uv_MainTexTwo;
            };

            sampler2D _MainTex;
            sampler2D _MainTexTwo;

            float4 _Color;
			float _TextureBlend;

            void surf(Input IN, inout SurfaceOutput o)
            {
				o.Albedo = lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_MainTexTwo, IN.uv_MainTexTwo), _TextureBlend) * _Color;
            }

            ENDCG
        }

        Fallback "Diffuse"
}