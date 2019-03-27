// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Grid Shader"
{
	Properties
	{
		_GridColor("Color", Color) = (1,.1,0,1)
		_Rows("Rows", float) = 3
		_Columns("Columns", float) = 4
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
			float4 _GridColor;
			float4 _OutlineColor;
			float4 _MainTex_ST;
			float _RowWidth;
			float _ColumnWidth;
			float _Columns;
			float _RowOffset;
			float _Rows;
			
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

				//Draw rows
				float rowWidth = _RowWidth;
				float rows = _Rows - 1;
				float rowStep = (1 / rows);
				
				y *= 100;
				rowStep *= 100;

				if (y % rowStep <= rowWidth/2 || y % rowStep >= rowStep - rowWidth/2)
				{
					return _OutlineColor;
				}

				//Draw columns
				float columnWidth = _ColumnWidth;
				float columns = _Columns - 1;
				float columnStep = (1 / columns);

				x *= 100;
				columnStep *= 100;

				if (x % columnStep <= columnWidth / 2 || x % columnStep >= columnStep - columnWidth / 2)
				{
					return _OutlineColor;
				}

				//float offset = _RowOffset;
				//float rowWidth = _RowWidth;
				//float columnWidth = _ColumnWidth;
				//float row = y *_Rows*2;
				//float column = x* _Columns;

				//row -= rowWidth/2;

				//if (row % 2 > (2-rowWidth)) {
				//	return _OutlineColor;
				//}
				/*if (column % 2 > (2-rowWidth)) {
					return _OutlineColor;
				}*/
				return _GridColor;
			}
			ENDCG
		}
	}
}
