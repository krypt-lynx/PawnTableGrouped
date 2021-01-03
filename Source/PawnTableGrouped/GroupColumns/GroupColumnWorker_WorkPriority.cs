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
    class GroupColumnWorker_WorkPriority : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroup group, PawnTable table, int columnIndex)
        {
            if (!group.IsUniform(columnIndex))
            {
                DoMixedValuesWidget(rect, group, columnIndex);
            }
            else
            {
                var pawn = group.Pawns.First();
                GuiTools.PushFont(GameFont.Medium);
                float x = rect.x + (rect.width - 25f) / 2f;
                float y = rect.y + 2.5f;
                var incapable = false;
                WidgetsWork.DrawWorkBoxFor(x, y, pawn, ColumnDef.workType, incapable);
                CopyToGroup(pawn, group);
                Rect rect2 = new Rect(x, y, 25f, 25f);
                if (Mouse.IsOver(rect2))
                {
                    TooltipHandler.TipRegion(rect2, () => WidgetsWork.TipForPawnWorker(pawn, ColumnDef.workType, incapable), pawn.thingIDNumber ^ ColumnDef.GetHashCode());
                }
                GuiTools.PopFont();
            }
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return 3;
        }

        public override object GetValue(Pawn pawn)
        {
            if (pawn.WorkTypeIsDisabled(ColumnDef.workType))
            {
                return -1;
            }

            return pawn.workSettings.GetPriority(ColumnDef.workType);
        }


        public override bool IsVisible(Pawn pawn)
        {
            return !pawn.WorkTypeIsDisabled(ColumnDef.workType);
        }

        public override void SetValue(Pawn pawn, object value)
        {
            var priority = (int)value;
            if (priority >= 0)
            {
                if (!pawn.WorkTypeIsDisabled(ColumnDef.workType))
                {
                    pawn.workSettings.SetPriority(ColumnDef.workType, priority);
                }
            }
        }
    }
}
