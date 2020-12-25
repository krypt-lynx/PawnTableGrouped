using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WildlifeTabAlt
{
    class CPawnListHeader : CElement
    {
        private PawnTable_WildlifeGrouped table;
        private PawnTableAccessor accessor;


        public CPawnListHeader(PawnTable_WildlifeGrouped pawnTable_WildlifeGrouped, PawnTableAccessor accessor)
        {
            this.table = pawnTable_WildlifeGrouped;
            this.accessor = accessor;
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(accessor.cachedHeightNoScrollbar, accessor.cachedHeaderHeight);
        }

        public override void DoContent()
        {
            base.DoContent();
            var columns = table.ColumnsListForReading;


            float width = BoundsRounded.width - 16f;
            int x = 0;
            for (int headerColumnIndex = 0; headerColumnIndex < columns.Count; headerColumnIndex++)
            {
                int columnWidth;
                if (headerColumnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(width - x);
                }
                else
                {
                    columnWidth = (int)accessor.cachedColumnWidths[headerColumnIndex];
                }
                Rect rect = new Rect(BoundsRounded.xMin + x, BoundsRounded.yMin, columnWidth, BoundsRounded.height);
                columns[headerColumnIndex].Worker.DoHeader(rect, table);
                x += columnWidth;
            }
        }

    }
}
