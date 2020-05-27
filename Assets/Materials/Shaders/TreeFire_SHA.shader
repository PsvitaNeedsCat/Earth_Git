Shader "Earth/Tree Fire"
{
    Properties
    {
        [Header(Main Texture)]
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0, 1)) = 0.5
        _ShadowThresh("Shadow Threshold", Range(0, 1)) = 0.3
        _ShadowSmooth("Shadow Smooth", Range(0, 1)) = 0.5
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
        [Space]
        [Header(Interior)]
        _MaskTex("Mask", 2D) = "white" {}
        [HDR]_InteriorColor("Interior Color", Color) = (1, 1, 1, 1)
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _ScrollX("Horizontal Scroll", Float) = 0.0
        _ScrollY("Vertical Scroll", Float) = 0.0
    }

        SubShader
        {
            Tags {"Queue" = "Opaque" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
        LOD 300
        Cull Off

        CGPROGRAM
        #pragma surface surf SoftLight alphatest:_Cutoff
        #pragma target 2.0

        half _ShadowThresh;
        half3 _ShadowColor;
        half _ShadowSmooth;

            half4 LightingSoftLight(SurfaceOutput s, half3 lightDir, half atten)
            {
                half d = pow(dot(s.Normal, lightDir) * 0.5, _ShadowThresh);
                half shadow = smoothstep(0.5, 0.5, d);
                half3 shadowCol = lerp(_ShadowColor, half3(1, 1, 1), shadow);
                half4 c;

                c.rgb = s.Albedo * atten * shadowCol;
                c.a = s.Alpha;

                return c;
            }

            sampler2D _MainTex;
            fixed4 _Color;

            struct Input
            {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = col.rgb;
                o.Alpha = col.a;
            }
            ENDCG

               CGPROGRAM
                #pragma surface surf Flat alphatest:_Cutoff
                #pragma target 2.0

                    half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
                {
                    return half4(o.Albedo, 1);
                }

                sampler2D _NoiseTex;
                sampler2D _MaskTex;
                fixed4 _InteriorColor;

                float _ScrollX;
                float _ScrollY;

                struct Input
                {
                    float2 uv_NoiseTex;
                    float2 uv_MaskTex;
                };

                void surf(Input IN, inout SurfaceOutput o)
                {
                    float uvX = IN.uv_NoiseTex.x + (_ScrollX * _Time.y);
                    float uvY = IN.uv_NoiseTex.y + (_ScrollY * _Time.y);
                    float2 uv = float2(uvX, uvY);

                    fixed4 mask = tex2D(_MaskTex, IN.uv_MaskTex);
                    fixed3 col = _InteriorColor.rgb;

                    o.Albedo = col.rgb;
                    o.Alpha = mask.a;
                    o.Emission = tex2D(_NoiseTex, uv).rgb;
                }
                ENDCG
        }

            FallBack "Particles/Standard Unlit"
}
