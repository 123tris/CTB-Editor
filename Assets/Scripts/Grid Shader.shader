// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Grid Shader"
{
	Properties
	{
		_RectSize("Rect size",Vector) = (100,100,0,0)
		_GridColor("Color", Color) = (1,.1,0,1)
		_Rows("Rows", float) = 3
		_Columns("Columns", float) = 4
		_RowOffset("Row offset",float) = 0.2
		_HitIndicatorWidth("Hit indicator width",float) = 1
		_HitIndicatorColor("Hit indicator color",Color) = (0,1,1,1)
		_RowWidth("Row width",float) = 1
		_ColumnWidth("Column width",float) = 1
		_ColumnColor("Column Color", Color) = (0, 0, 0, 1)
		_BeatsnapDivision("Beatsnap divisor",int) = 1
		_WholeOutlineColor("1/1 Outline color", Color) = (1,1,1,0)
		_HalfOutlineColor("1/2 Outline color", Color) = (1,0,0,0)
		_QuaterOutlineColor("1/4 Outline color", Color) = (0,0,1,0)
		_EighthOutlineColor("1/8 Outline color", Color) = (1,1,0,0)
		_SixteenthOutlineColor("1/16 Outline color", Color) = (.5,.5,.5,0)
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
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
			int _BeatsnapDivision;
			float4 _WholeOutlineColor;
			float4 _HalfOutlineColor;
			float4 _QuaterOutlineColor;
			float4 _EighthOutlineColor;
			float4 _SixteenthOutlineColor;
			float4 _MainTex_ST;
			float4 _RectSize;
			float4 _HitIndicatorColor;
			float _HitIndicatorWidth;
			float _RowWidth;
			float _ColumnWidth;
			float _Columns;
			float4 _ColumnColor;
			float _RowOffset;
			float _Rows;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.localPos = v.vertex;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float x = i.uv.x;
				float y = i.uv.y;

				//Draw rows
				float rowWidth = _RowWidth;
				float rows = _Rows;
				float rowStep = (1 / rows);

				y *= _RectSize.y; //Multiply the Y value of the UV with height of the window so that the integer can represent the individual pixel and there is no rounding
				rowStep *= _RectSize.y;

				y += _RowOffset;

				if (y < -_HitIndicatorWidth/2)
				{
					return float4(0,0,0,0);
				}

				if (y % rowStep < rowWidth / 2 || y % rowStep > rowStep - rowWidth / 2)
				{
					float4 lineColor;
					uint beatsnapDivision = _BeatsnapDivision;
					uint currentLine = (((round(y) + 1) / rowStep) % beatsnapDivision) + 1;
					if (beatsnapDivision == 16)
					{
						if (currentLine % 2 == 0) return _SixteenthOutlineColor;
						if (currentLine % 9 == 0) return _HalfOutlineColor;
						if (currentLine % 7 == 0 || currentLine % 3 == 0 || currentLine % 11 == 0) return _EighthOutlineColor;
						if (currentLine % 5 == 0 || currentLine % 13 == 0) return _QuaterOutlineColor;
					}

					if (beatsnapDivision == 8)
					{
						if (currentLine % 2 == 0) return _EighthOutlineColor;
						if (currentLine % 7 == 0 || currentLine % 3 == 0) return _QuaterOutlineColor;
						if (currentLine % 5 == 0) return _HalfOutlineColor;
					}
					if (beatsnapDivision == 4)
					{
						if (currentLine % 2 == 0) return _QuaterOutlineColor;
						if (currentLine % 3 == 0) return _HalfOutlineColor;
					}
					if (beatsnapDivision == 2 && currentLine % 2 == 0) return _HalfOutlineColor;
					return _WholeOutlineColor;
				}

				//Draw columns
				float columnWidth = _ColumnWidth;
				float columns = _Columns;
				float columnStep = (1 / columns);

				x *= _RectSize.x;
				columnStep *= _RectSize.x;

				if (x % columnStep < columnWidth / 2 || x % columnStep > columnStep - columnWidth / 2)
				{
					return _ColumnColor;
				}

				//Draw hitindicator
				y -= _RowOffset;
				if (abs(y - (_RectSize.y / 10)) < _HitIndicatorWidth)
				{
					return _HitIndicatorColor;
				}

				return _GridColor;
			}
			ENDCG
		}
	}
}
