Shader "Earth/Unlit Character Blend No Silhouette"
{
    Properties
    {
        [Header(MAIN TEXTURE)]
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Texture One", 2D) = "white" {}
		_MainTexTwo("Texture Two", 2D) = "white" {}
		_TextureBlend("Texture Blend", Range(0,1)) = 0.0
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

            struct Input
            {
                float2 uv;
            };

            ENDCG


            ZWrite On
            ZTest LEqual
            Cull Off

            CGPROGRAM
            #pragma target 3.0

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