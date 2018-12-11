/// Credit John Hattan (http://thecodezone.com/)
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/117/uigridrenderer


namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UIGridRenderer")]
	public class UIGridRenderer : UILineRenderer
	{
		[SerializeField]
		private float m_GridColumns = 10;
		[SerializeField]
		private float m_GridRows = 10;

        public int offset = 0;
		/// <summary>
		/// Number of columns in the Grid
		/// </summary>
        public float GridColumns
		{
			get
			{
				return m_GridColumns;
			}

			set
			{
				if (m_GridColumns == value)
					return;
				m_GridColumns = value;
				SetAllDirty();
			}
		}

		/// <summary>
		/// Number of rows in the grid.
		/// </summary>
        public float GridRows
		{
			get
			{
				return m_GridRows;
			}

			set
			{
				if (m_GridRows == value)
					return;
				m_GridRows = value;
				SetAllDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			relativeSize = true;

            int ArraySize = (int)Mathf.Ceil(GridRows) * 3 + 2;
			if(GridRows % 2 == 0)
				++ArraySize; // needs one more line

			ArraySize += (int)Mathf.Ceil(GridColumns) * 3 + 1;

			m_points = new Vector2[ArraySize];

            float relativePixelHeight = 1 / rectTransform.rect.height;
            int Index = 0;
            float yDistance = rectTransform.rect.height / GridRows;
            offset %= (int)yDistance;


            for (int i = 0; i < GridRows; ++i)
            { 
				float xFrom = 1;
				float xTo = 0;
				if(i % 2 == 0)
				{
					// reach left instead
					xFrom = 0;
					xTo = 1;
				}
                float softOffset = i == 0 ? 0 : offset;

                float y = i / GridRows;
				m_points[Index].x = xFrom;
				m_points[Index].y = y - relativePixelHeight * softOffset;
				++Index;
				m_points[Index].x = xTo;
				m_points[Index].y = y - relativePixelHeight * softOffset;
				++Index;
				m_points[Index].x = xTo;
				m_points[Index].y = Mathf.Min((i + 1) / GridRows - relativePixelHeight * softOffset, 1);
				++Index;
			}

            int additionSize = (int)(yDistance - (GridRows - Mathf.Floor(GridRows)) / GridRows * rectTransform.rect.height);
           
                
            if (offset >= additionSize)
            {
                if (Mathf.Floor(GridRows) % 2 == 0)
                {
                    m_points[Index].x = 0;
                    m_points[Index].y = 1 - relativePixelHeight * (offset - additionSize);
                    ++Index;
                    m_points[Index].x = 0;
                    m_points[Index].y = 1;
                    ++Index;
                    m_points[Index].x = 1;
                    m_points[Index].y = 1;
                    ++Index;
                    m_points[Index].x = 0;
                    m_points[Index].y = 1;
                    ++Index;
                } else
                {
                    m_points[Index].x = 1;
                    m_points[Index].y = 1 - relativePixelHeight * (offset - additionSize);
                    ++Index;
                    m_points[Index].x = 0;
                    m_points[Index].y = 1;
                    ++Index;
                }
            }
            else
            {
                float xFrom = 0;
                float xTo = 1;

                if (Mathf.Floor(GridRows) % 2 == 0)
                {
                    // reach left instead
                    xFrom = 1;
                    xTo = 0;
                }

                m_points[Index].x = xTo;
                m_points[Index].y = 1;
                ++Index;
                m_points[Index].x = xFrom;
                m_points[Index].y = 1;
                ++Index;
                m_points[Index].x = xFrom;
                m_points[Index].y = 0;
                ++Index;

            }
			// line is now at 0,1, so we can draw the columns
			for(int i = 0; i < GridColumns; ++i)
			{
				float yFrom = 1;
				float yTo = 0;
				if(i % 2 == 0)
				{
					// reach up instead
					yFrom = 0;
					yTo = 1;
				}

				float x = i / GridColumns;
				m_points[Index].x = x;
				m_points[Index].y = yFrom;
				++Index;
				m_points[Index].x = x;
				m_points[Index].y = yTo;
				++Index;
				m_points[Index].x = (i + 1) / GridColumns;
				m_points[Index].y = yTo;
				++Index;

              
			}

			if(GridColumns % 2 == 0)
			{
				// one more line to get to 1, 1
				m_points[Index].x = 1;
				m_points[Index].y = 1;
			}
			else
			{
				// one more line to get to 1, 0
				m_points[Index].x = 1;
				m_points[Index].y = 0;
			}

			base.OnPopulateMesh(vh);
		}
	}
}