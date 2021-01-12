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
    class CPawnListRow : CPawnTableRow
    {
        private PawnTable table;
        private PawnTableAccessor accessor;
        private Pawn pawn;
        PawnTableGroup group;
        private LookTargets target;
        private object[] oldValues = null;

        public CPawnListRow(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group, Pawn pawn)
        {
            this.table = table;
            this.accessor = accessor;
            this.pawn = pawn;
            this.group = group;
            this.target = new LookTargets(pawn);
            oldValues = new object[group.ColumnResolvers.Count];
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

            bool needUpdateSectionHeader = false;
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



                columns[columnIndex].Worker.DoCell(cellRect, pawn, table);

                
                object cellValue = group.GetValue(columnIndex, pawn);

                if (!object.Equals(cellValue, oldValues[columnIndex]))
                {
                    oldValues[columnIndex] = cellValue;
                    needUpdateSectionHeader = true;
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
