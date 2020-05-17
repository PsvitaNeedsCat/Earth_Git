Shader "Earth/Tree Fire"
{
    Properties
    {
        _Color("Base Colour", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Noise1("Noise 1 Texture", 2D) = "white" {}
        _Noise2("Noise 1 Texture", 2D) = "white" {}
        _ScrollX1("Scroll X", Float) = 0.0
        _ScrollY1("Scroll Y", Float) = 0.0
        _ScrollX2("Scroll X", Float) = 0.0
        _ScrollY2("Scroll Y", Float) = 0.0
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
            float2 uv_Noise1;
            float2 uv_Noise2;
        };

        sampler2D _MainTex;
        float4 _Color;

        sampler2D _Noise1;
        sampler2D _Noise2;

        fixed _ScrollX1;
        fixed _ScrollY1;

        fixed _ScrollX2;
        fixed _ScrollY2;

        void surf(Input IN, inout SurfaceOutput o)
        {
            float uvX1 = IN.uv_Noise1.x + (_ScrollX1 * _Time.y);
            float uvY1 = IN.uv_Noise1.y + (_ScrollY1 * _Time.y);

            float uvX2 = IN.uv_Noise2.x + (_ScrollX2 * _Time.y);
            float uvY2 = IN.uv_Noise2.y + (_ScrollY2 * _Time.y);

            float2 uv1 = float2(uvX1, uvY1);

            float2 uv2 = float2(uvX2, uvY2);

            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
            o.Emission = tex2D(_Noise1, uv1).rgb * tex2D(_Noise2, uv2).rgb;
        }


        ENDCG
        }
    Fallback "Diffuse"
}
