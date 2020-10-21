Shader "Unlit/Clouds_SHA"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Colour", Color) = (1, 1, 1, 1)
		_ScrollSpeed ("Scroll Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _ScrollSpeed;
			float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			float2 moveUV(float2 uv, float time)
			{
				return float2(uv.x, uv.y + time);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float2 newUV = moveUV(i.uv, _Time.w * _ScrollSpeed);

                // sample the texture
                fixed4 col = tex2D(_MainTex, newUV) * _Color;
                return col;
            }
            ENDCG
        }
    }
}
