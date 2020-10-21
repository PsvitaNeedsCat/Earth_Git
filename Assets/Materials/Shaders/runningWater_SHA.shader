Shader "Earth/Running Water"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _EmissionColor("Emission Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
            Cull Off
			

            CGPROGRAM
            #pragma surface surf Flat



        fixed4 LightingFlat(SurfaceOutput o, half3 lightDir, half atten)
            {
                return fixed4(o.Albedo, 1);
            }

    struct Input
    {
        float2 uv_MainTex;
    };

    float2 moveUV(float2 uv, float time)
    {
        return float2(uv.x, uv.y + time);
    }

    sampler2D _MainTex;
    fixed4 _Color;
    fixed4 _EmissionColor;

    void surf(Input IN, inout SurfaceOutput o)
    {
        float2 uv = moveUV(IN.uv_MainTex, _Time.y);

        o.Albedo = tex2D(_MainTex, uv).rgb * _Color;
		
        o.Emission = _EmissionColor;
    }

            
            ENDCG
        }
    Fallback"Diffuse"
}
