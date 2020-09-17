Shader "Earth/Gradient_SHA"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.3
        _Color("Colour Base 1", Color) = (1, 1, 1, 1)
        _Color1("Colour Base 2", Color) = (0, 0, 0, 1)
        _BlendAmount("Blend Percent", Range(0.0, 1.0)) = 0.0
        _Dist("Distortion", Float) = 0.1
        _Scroll("Scroll Speed", Float) = 0.1
        _BlendTex ("Blend Texture (Noise)", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Flat alphatest:_Cutoff vertex:vert

        half4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
            {
                return half4(o.Albedo, 1);
            }

        struct Input
        {
            float2 uv_MainTex;
            float2 st_BlendTex;
        };

        sampler2D _BlendTex;
        float4 _BlendTex_ST;

        float _Dist;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.st_BlendTex = TRANSFORM_TEX(v.texcoord, _BlendTex);

            o.st_BlendTex.x += sin((o.st_BlendTex.x + o.st_BlendTex.y) + _Time.g) * _Dist;
            o.st_BlendTex.y += cos((o.st_BlendTex.x - o.st_BlendTex.y) + _Time.g) * _Dist;
        }

        sampler2D _MainTex;
       // sampler2D _BlendTex;

        float3 _Color;
        float3 _Color1;
        float _BlendAmount;
        float _Scroll;

        float2 moveUV(float2 uv, float time)
        {
            return float2(uv.x - time, uv.y);
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 uv = moveUV(IN.st_BlendTex, frac(_Time.y * _Scroll));

            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);

            fixed3 gra = lerp(_Color, _Color1, _BlendAmount * tex2D(_BlendTex, uv));

            o.Albedo = col.rgb * gra;
            o.Alpha = col.a;
        }

        ENDCG
    }
}
