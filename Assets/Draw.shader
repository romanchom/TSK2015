Shader "HiddenDraw"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	float4x4 _Matrix;
	fixed4 _Color;

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

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(_Matrix, v.vertex);
		o.vertex = mul(UNITY_MATRIX_MVP, o.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f i) : SV_Target
	{
		//fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
		fixed4 tex = tex2D(_MainTex, i.uv);
		fixed4 col = _Color;
		return fixed4(col.r, col.g, col.b, tex.a * col.a);
	}

	ENDCG

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha One
		//Blend One OneMinusSrcColor
		//Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0.01

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
