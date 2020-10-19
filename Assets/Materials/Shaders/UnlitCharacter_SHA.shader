Shader "Earth/Unlit Character"
{
    Properties
    {
        [Header(MAIN TEXTURE)]
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
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
            };

            sampler2D _MainTex;

            float4 _Color;

            void surf(Input IN, inout SurfaceOutput o)
            {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
            }

            ENDCG


        }

        Fallback "Unlit/Texture"
}