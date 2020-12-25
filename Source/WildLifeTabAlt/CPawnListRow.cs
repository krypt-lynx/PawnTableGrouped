using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt
{
    class CPawnListRow : CListingRow
    {
        private PawnTable_WildlifeGrouped table;
        private PawnTableAccessor accessor;
        private Pawn pawn;
        private LookTargets target;

        public CPawnListRow(PawnTable_WildlifeGrouped pawnTable_WildlifeGrouped, PawnTableAccessor accessor, Pawn pawn)
        {
            this.table = pawnTable_WildlifeGrouped;
            this.accessor = accessor;
            this.pawn = pawn;
            this.target = new LookTargets(pawn);
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(accessor.cachedHeightNoScrollbar, accessor.CalculateRowHeight(pawn));
        }

        public override void DoContent()
        {
            base.DoContent();

            int x = (int)BoundsRounded.xMin;
            var columns = table.ColumnsListForReading;


            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.yMin, BoundsRounded.width);
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
                columns[columnIndex].Worker.DoCell(cellRect, pawn, table);
                x += columnWidth;
            }
            if (pawn.Downed)
            {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.center.y, BoundsRounded.width);
            }
            GUI.color = Color.white;
        }

        /*
                    row.Embed(row.AddElement(new CWidget
                    {
                        DoWidgetContent = (_, bounds) => {
                            int x = (int)bounds.xMin;

                            GUI.color = new Color(1f, 1f, 1f, 0.2f);
                            Widgets.DrawLineHorizontal((int)bounds.xMin, bounds.yMin, bounds.width);
                            GUI.color = Color.white;
                            if (!this.CanAssignPawn(pawn))
                            {
                                GUI.color = Color.gray;
                            }

                            if (Mouse.IsOver(bounds))
                            {
                                GUI.DrawTexture(bounds, TexUI.HighlightTex);
                                //this.cachedLookTargets[rowIndex].Highlight(true, this.cachedPawns[rowIndex].IsColonist, false);
                            }
                            for (int columnIndex = 0; columnIndex < ColumnsListForReading.Count; columnIndex++)
                            {
                                int columnWidth;
                                if (columnIndex == ColumnsListForReading.Count - 1)
                                {
                                    columnWidth = (int)(width - x);
                                }
                                else
                                {
                                    columnWidth = (int)accessor.cachedColumnWidths[columnIndex];
                                }
                                Rect cellRect = new Rect(x, bounds.yMin, columnWidth, (int)rowHeight);
                                ColumnsListForReading[columnIndex].Worker.DoCell(cellRect, pawn, this);
                                x += columnWidth;
                            }
                            if (pawn.Downed)
                            {
                                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                                Widgets.DrawLineHorizontal(bounds.xMin, bounds.center.y, bounds.width);
                            }
                            GUI.color = Color.white;
                        }

                    }));
        */
    }
}
