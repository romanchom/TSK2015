Shader "Unlit/TextureUnlitBicolor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorPos ("Positive Color", Color) = (1, 0, 0, 0)
		_ColorNeg ("Negative Color", Color) = (0, 1, 0, 0)
		_Scale ("Scale", Float) = 1 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			float4 _ColorPos;
			float4 _ColorNeg;
			float _Scale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float col = tex2D(_MainTex, i.uv).r;
				col *= _Scale;
				return max(col, 0) * _ColorPos + max(-col, 0) * _ColorNeg;
			}
			ENDCG
		}
	}
}
