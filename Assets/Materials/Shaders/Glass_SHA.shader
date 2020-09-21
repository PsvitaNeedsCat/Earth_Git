// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Earth/Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Colour", Color) = (1, 1, 1, 1)
            [Space]
        [Header(DISTORTION)]
        _Strength("Distortion Strength", Float) = 1.0
    }
    SubShader
    {
        Tags
        { "RenderType"="Transparent"
            "DisableBatching" = "True"
        }

        LOD 100

		GrabPass{}

			/*Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

        struct v2f {
            float2 uv: TEXCOORD0;
			UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
            float4 screenUV : TEXCOORD1;
        };

		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _GrabTexture
			;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			UNITY_TRANSFER_FOG(o, o.vertex);

			o.screenUV = ComputeGrabScreenPos(o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target{
			fixed grab = tex2Dproj(_GrabTexture, i.screenUV);
		return grab;
		}
        

            ENDCG
        }*/

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
			float4 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = ComputeGrabScreenPos(o.vertex);
			return o;
		}

		sampler2D _GrabTexture;

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = tex2Dproj(_GrabTexture, i.uv);
		return col;
		}

			ENDCG
}
    }
}
