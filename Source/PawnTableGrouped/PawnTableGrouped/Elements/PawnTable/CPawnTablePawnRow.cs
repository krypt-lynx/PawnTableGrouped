using PawnTableGrouped.TableGrid;
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
    class CPawnTablePawnRow : ICTableGridRow
    {
        private PawnTableWrapper table;
        private PawnTableGroup group;
        private Pawn pawn;
        private LookTargets target;

        private object[] oldValues;

        bool needUpdateSectionHeader = false;

        public CPawnTablePawnRow(PawnTableWrapper table, PawnTableGroup group, Pawn pawn)
        {
            this.table = table;
            this.group = group;
            this.pawn = pawn;
            target = new LookTargets(pawn);

            oldValues = new object[group.ColumnResolvers.Count];
        }

        public bool CanCombineWith(ICTableGridRow other, int columnIndex)
        {
#if rw_1_4_or_later
            if (other is CPawnTablePawnRow otherPawnRow)
            {
                return table.Columns[columnIndex].Worker.CanGroupWith(pawn, otherPawnRow.pawn);
            } else
            {
                return false;
            }
#else
            return false;
#endif
        }

        public void DoBackground(Rect rect) {
            rect = rect.ContractedBy(new EdgeInsets(0, 0, 0, Metrics.TableLeftMargin));

            // decorative line
#if rw_1_3_or_earlier
            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(rect.xMin, rect.yMin, rect.width);
            GUI.color = Color.white;
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, TexUI.HighlightTex);
                target.Highlight(true, pawn.IsColonist, false);
            }
#endif
            needUpdateSectionHeader = false;
        }

        public void DoCell(Rect rect, int columnIndex, bool hightlighted, bool combined)
        {
            if (columnIndex == 0)
            {
                rect = rect.ContractedBy(new EdgeInsets(0, 0, 0, Metrics.TableLeftMargin));
            }
#if rw_1_4_or_later
            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(rect.xMin, rect.yMin, rect.width);
            if (combined)
            {
                Widgets.DrawLineVertical(rect.xMin, rect.yMin, rect.height);
                Widgets.DrawLineVertical(rect.xMax, rect.yMin, rect.height);
            }
            GUI.color = Color.white;
#endif

            table.Columns[columnIndex].Worker.DoCell(rect, pawn, table.Table);

#if rw_1_4_or_later
            if (hightlighted)
            {
                GUI.DrawTexture(rect, TexUI.HighlightTex);
                target.Highlight(true, pawn.IsColonist, false);
            }
#endif

            if (!Mod.Settings.disableGroupCells) // todo: check if dummy
            {
                object cellValue = group.GetValue(columnIndex, pawn);

                if (!object.Equals(cellValue, oldValues[columnIndex]))
                {
                    oldValues[columnIndex] = cellValue;
                    needUpdateSectionHeader = true;
                }
            }

            //GuiTools.PushColor(Color.green);
            //GuiTools.Box(rect, EdgeInsets.One);
            //Widgets.Label(rect, columnIndex.ToString());
            //GuiTools.PopColor();
        }

        public void DoOverlay(Rect rect)
        {
            rect = rect.ContractedBy(new EdgeInsets(0, 0, 0, Metrics.TableLeftMargin));

            if (pawn.Downed)
            {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(rect.xMin, rect.center.y, rect.width);
            }

            if (needUpdateSectionHeader)
            {
                group.NotifyValueChanged(); // todo: section header uodate in the same frame
            }
        }

        public float GetRowHeight()
        {
            return table.CalculateRowHeight(pawn);
        }
    }

}
