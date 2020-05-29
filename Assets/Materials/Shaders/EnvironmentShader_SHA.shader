Shader "Earth/Environment"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0, 1)) = 0.5
        _ShadowThresh("Shadow Threshold", Range(0, 1)) = 0.3
        _ShadowSmooth("Shadow Smooth", Range(0, 1)) = 0.5
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
    }

        SubShader
    {
        Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
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
            sampler2D _AlphaTex;
            fixed4 _Color;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_AlphaTex;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = col.rgb;
                o.Alpha = col.a;
            }
            ENDCG
        }

     FallBack "Particles/Standard Unlit"
}
