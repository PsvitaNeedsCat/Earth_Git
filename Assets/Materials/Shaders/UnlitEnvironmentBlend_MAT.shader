Shader "Earth/Unlit Environment Blend"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture One", 2D) = "white" {}
        _MainTexTwo("Texture Two", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0, 1)) = 0.5
		_TextureBlend("Texture Blend", Range(0,1)) = 0.0
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

            sampler2D _MainTex;
            sampler2D _MainTexTwo;
            sampler2D _AlphaTex;
            fixed4 _Color;
			float _TextureBlend;

            struct Input
            {
                float2 uv_MainTex;
				float2 uv_MainTexTwo;
                float2 uv_AlphaTex;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
				fixed4 col = lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_MainTexTwo, IN.uv_MainTexTwo), _TextureBlend) * _Color;

                o.Albedo = col.rgb;
                o.Alpha = col.a;
            }
            ENDCG
        }

     FallBack "Particles/Standard Unlit"
}
