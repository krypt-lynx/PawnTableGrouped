using Cassowary;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{

    class CPawnListGroupSummary : CRowSegment
    {

        private PawnTable table;
        private PawnTableAccessor accessor;
        PawnTableGroup Group;

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(0, Metrics.GroupHeaderHeight);
        }

        public CPawnListGroupSummary(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group)
        {
            this.table = table;
            this.accessor = accessor;
            this.Group = group;
        }

        public override void DoContent()
        {
            base.DoContent();

            DoRowsSummary();

            if (Widgets.ButtonInvisible(BoundsRounded))
            {
                (Row as CPawnListGroupRow)?.Action?.Invoke((CPawnListGroupRow)Row);
            }
        }

        private void DoRowsSummary()
        {
            int x = (int)BoundsRounded.xMin;
            var columns = table.ColumnsListForReading;

            var overflow = (Row as CPawnListGroupRow)?.overflow ?? 0;

            for (int columnIndex = 1; columnIndex < columns.Count; columnIndex++)
            {
                int columnWidth;
                if (columnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(BoundsRounded.width - x);
                }
                else
                {
                    columnWidth = (int)accessor.cachedColumnWidths[columnIndex];
                }

                if (x >= xScrollOffset + overflow && x <= xScrollOffset + visibleRectWidth)
                {
                    var resolver = Group.ColumnResolvers[columnIndex];
                    if (resolver != null && Group.IsVisible(columnIndex))
                    {
                        Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, BoundsRounded.height);
                        resolver.DoCell(cellRect, Group.Columns[columnIndex], table);
                    }
                }
                x += columnWidth;
            }
            /*if (pawn.Downed)
            {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.center.y, BoundsRounded.width);
            }*/
            GUI.color = Color.white;
        }
    }   
}
