// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Earth/Glass"
{
	Properties
	{
		_MainTex("Main Texure", 2D) = "white"{}
		_Color("Base Colour", Color) = (1, 1, 1, 1)
			_Color2("Texture Color", Color) = (1, 1, 1 ,1)
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			//Transparency

			Lighting Off
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			//Tags {"Queue"="Transparent"}
			Color[_Color]
			Pass
			{
				Stencil{
					Ref 1	//Ref value
					Comp Greater	//If value greater
					Pass IncrSat	//Increase saturation
				}
			}

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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color2;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color2;
			return col;
		}
		ENDCG
	}

		}
}