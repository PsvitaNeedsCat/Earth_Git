Shader "Earth/Unlit Environment"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0, 1)) = 0.5
    }

        SubShader
    {
        Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
        Cull Off
        LOD 300

        CGPROGRAM
        #pragma surface surf Flat alphatest:_Cutoff
        #pragma target 2.0

        half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
        {
            return half4(o.Albedo, 1);
        }
        s
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
