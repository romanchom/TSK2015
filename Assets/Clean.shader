Shader "Hidden/Clean"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	float4x4 _Matrix;

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
		fixed4 col = tex2D(_MainTex, i.uv);
		return fixed4(0, 0, 0, col.a);
	}

	ENDCG

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend Zero OneMinusSrcAlpha
		//ColorMask A

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
