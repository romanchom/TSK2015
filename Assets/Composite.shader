﻿Shader "Hidden/Composite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScreenTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _ScreenTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 screen = tex2D(_ScreenTex, i.uv);
				fixed4 main = tex2D(_MainTex, i.uv);
				return fixed4(main.rgb * (1 - screen.a) + screen.rgb * screen.a, 1);
			}
			ENDCG
		}
	}
}
