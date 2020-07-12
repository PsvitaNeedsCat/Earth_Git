Shader "Earth/Wave_SHA"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowTex("Shading Texture", 2D) = "white" {}
        _ScrollX ("Scroll X", Float) = -0.5
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

            void surf(Input IN, inout SurfaceOutput o)
            {
                float uvX = IN.uv_MainTex.x + (_ScrollX * _Time.y);
                float2 newUV = float2(uvX, IN.uv_MainTex.y);
                fixed3 col = tex2D(_MainTex, newUV).rgb;

                fixed3 sha = tex2D(_ShadowTex, IN.uv_ShadowTex).rgb;

                o.Albedo = col * sha;
            }

            ENDCG
        }
        FallBack "Particles/Standard Unlit"
}
