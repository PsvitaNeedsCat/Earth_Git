Shader "Earth/Clouds_SHA"
{
    Properties
    {
		_Color("Colour", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Cloud Noise", 2D) = "white" {}
		_NoiseStrength("Noise Strength", float) = 0.5
		_NoiseSpeed ("Noise Speed", float) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

			sampler2D _NoiseTex;
			float _NoiseStrength;
			float _NoiseSpeed;

            struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
			};

			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;

				output.pos = UnityObjectToClipPos(input.vertex);

				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				output.pos.y += sin(_Time * _NoiseSpeed * noiseSample) * _NoiseStrength;
				output.pos.x += cos(_Time * _NoiseSpeed * noiseSample) * _NoiseStrength;

				return output;
			}
			float4 _Color;

			float4 frag(VertexOutput input) : COLOR
			{
				float4 col = _Color;

				return col;
			}

            ENDCG
        }
    }
}
