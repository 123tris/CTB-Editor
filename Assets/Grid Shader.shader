// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Brick Shader"
{
	Properties
	{
		_BrickColor("Color", Color) = (1,.1,0,1)
		_Rows("Rows", int) = 3
		_Columns("Columns", int) = 4
		_RowOffset("Row offset",float) = 0.2
		_RowWidth("Row width",float) = 1
		_ColumnWidth("Column width",float) = 1
		_OutlineColor("Outline color", Color) = (0,0,0,0)
		_MainTex ("Texture", 2D) = "white" {}
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
			#pragma multi_compile_fog
			
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
				float4 localPos : NONE;
			};

			//implement properties
			sampler2D _MainTex;
			float4 _BrickColor;
			float4 _OutlineColor;
			float4 _MainTex_ST;
			float _RowWidth;
			float _ColumnWidth;
			float _Columns;
			float _RowOffset;
			int _Rows;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.localPos = v.vertex;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float x = i.uv.x;
				float y = i.uv.y;

				float row = y*_Rows*2;
				float column = x* _Columns;
				float rowWidth = _RowWidth;
				float columnWidth = _ColumnWidth;
				float offset = _RowOffset;

				row -= rowWidth/2;

				if (row % 2 > (2-rowWidth)) {
					return _OutlineColor;
				}
				if (column % 2 > (2-rowWidth)) {
					return _OutlineColor;
				}

				return float4(0,0,0,255);
			}
			ENDCG
		}
	}
}
