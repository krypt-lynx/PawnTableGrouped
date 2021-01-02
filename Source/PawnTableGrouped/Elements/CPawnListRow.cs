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
    class CPawnListRow : CListingRow
    {
        private PawnTable table;
        private PawnTableAccessor accessor;
        private Pawn pawn;
        PawnTableGroup group;
        private LookTargets target;

        public CPawnListRow(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group, Pawn pawn)
        {
            this.table = table;
            this.accessor = accessor;
            this.pawn = pawn;
            this.group = group;
            this.target = new LookTargets(pawn);
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(accessor.cachedHeightNoScrollbar, accessor.CalculateRowHeight(pawn));
        }

        public override void DoContent()
        {
            base.DoContent();

            int x = (int)(BoundsRounded.xMin + Metrics.TableLeftMargin);
            var columns = table.ColumnsListForReading;
                        
            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(BoundsRounded.xMin + Metrics.TableLeftMargin, BoundsRounded.yMin, BoundsRounded.width - Metrics.TableLeftMargin);
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
            for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
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
                Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, (int)BoundsRounded.height);

                bool needValidateCellValue = Event.current.type != EventType.Repaint &&
                        Event.current.type != EventType.Layout &&
                        Event.current.type != EventType.Ignore &&
                        Event.current.type != EventType.Used && 
                        Mouse.IsOver(cellRect) &&
                        group.IsInteractive(columnIndex);

                object oldCellValue = null;
                if (needValidateCellValue)
                {
                    oldCellValue = group.GetValue(columnIndex, pawn);
                }

                columns[columnIndex].Worker.DoCell(cellRect, pawn, table);

                if (needValidateCellValue)
                {
                    object newCellValue = group.GetValue(columnIndex, pawn);

                    if (!object.Equals(oldCellValue, newCellValue))
                    {
                        group.NotifyValueChanged(columnIndex);
                    }
                }

                x += columnWidth;
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
