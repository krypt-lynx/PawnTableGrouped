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
    class CPawnListRow : CRowSegment
    {
        private PawnTable table;
        private PawnTableAccessor accessor;
        private Pawn pawn;
        private PawnTableGroup group;
        private LookTargets target;
        private object[] oldValues = null;
        private RangeInt columnsRange;
        private bool doLeftOffset;

        public CPawnListRow(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group, Pawn pawn, RangeInt columnsRange, bool doLeftOffset)
        {
            this.table = table;
            this.accessor = accessor;
            this.pawn = pawn;
            this.group = group;
            this.target = new LookTargets(pawn);
            this.columnsRange = columnsRange;
            this.doLeftOffset = doLeftOffset;
            oldValues = new object[group.ColumnResolvers.Count];
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(accessor.cachedHeightNoScrollbar, accessor.CalculateRowHeight(pawn));
        }

        public override void DoContent()
        {
            base.DoContent();

            var columns = table.ColumnsListForReading;
            var cachedColumnWidths = accessor.cachedColumnWidths;

            // decorative line
            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(BoundsRounded.xMin + (doLeftOffset ? Metrics.TableLeftMargin : 0), BoundsRounded.yMin, BoundsRounded.width - (doLeftOffset ? Metrics.TableLeftMargin : 0));
            GUI.color = Color.white;

            if (!accessor.CanAssignPawn(pawn))
            {
                GUI.color = Color.gray;
            }

            if (Mouse.IsOver(BoundsRounded))
            {
                GUI.DrawTexture(BoundsRounded, TexUI.HighlightTex);
                target.Highlight(true, pawn.IsColonist, false);
            }

            bool needUpdateSectionHeader = false;

            int x = (int)(BoundsRounded.xMin + (doLeftOffset ? Metrics.TableLeftMargin : 0));

            int start = Mathf.Max(0, columnsRange.start);
            int end = Mathf.Min(columns.Count, columnsRange.end);
            for (int columnIndex = start; columnIndex < end; columnIndex++)
            {
                int columnWidth;
                if (columnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(BoundsRounded.width - x);
                }
                else
                {
                    columnWidth = (int)cachedColumnWidths[columnIndex];
                } 

                if (x + columnWidth > xScrollOffset && x <= xScrollOffset + visibleRectWidth)
                {
                    Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, BoundsRounded.height);

                    columns[columnIndex].Worker.DoCell(cellRect, pawn, table);

                    object cellValue = group.GetValue(columnIndex, pawn);

                    if (!object.Equals(cellValue, oldValues[columnIndex]))
                    {
                        oldValues[columnIndex] = cellValue;
                        needUpdateSectionHeader = true;
                    }
                }

                x += columnWidth;
            }

            if (needUpdateSectionHeader)
            {
                group.NotifyValueChanged(); // todo: section header uodate in the same frame
            }

            if (pawn.Downed)
            {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.center.y, BoundsRounded.width);
            }
            GUI.color = Color.white;
        }
    }
}
