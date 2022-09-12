// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Displacement"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DisplaceTex("Displacement Texture", 2D) = "white" {}
		_DisplaceTex2("Displacement Texture 2", 2D) = "white" {}
		//_BlurMagnitude("Blur Magnitude", Int) = 1
		_Magnitude("Magnitude", Range(0,0.1)) = 1
			_Magnitude2("Magnitude 2", Range(0,0.1)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _DisplaceTex;
			sampler2D _DisplaceTex2;
			float _Magnitude;
			float _Magnitude2;
			//int _BlurMagnitude;
			float4 _MainTex_TexelSize;

			/*float4 box(sampler2D tex, float2 uv, float4 size)
			{
				float4 c;

				for(int i = 0; i < _BlurMagnitude; i++)
					c = tex2D(tex, uv + float2(-size.x, size.y)) + tex2D(tex, uv + float2(0, size.y)) + tex2D(tex, uv + float2(size.x, size.y)) +
						tex2D(tex, uv + float2(-size.x, 0)) + tex2D(tex, uv + float2(0, 0)) + tex2D(tex, uv + float2(size.x, 0)) +
						tex2D(tex, uv + float2(-size.x, -size.y)) + tex2D(tex, uv + float2(0, -size.y)) + tex2D(tex, uv + float2(size.x, -size.y));

				return c / 9;
			}*/

			float4 frag (v2f i) : SV_Target
			{
				float2 disp = tex2D(_DisplaceTex, i.uv).xy;
				disp = ((disp * 2) - 1) * _Magnitude;

				float disp2 = tex2D(_DisplaceTex2, i.uv).xy;
				disp2 = ((disp2 * 2) - 1) * _Magnitude2;

				float4 col = tex2D(_MainTex, i.uv + disp + disp2);
				//float4 col = box(_MainTex, i.uv + disp, _MainTex_TexelSize);
				return col;
			}
			ENDCG
		}
	}
}
