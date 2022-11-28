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
#if rw_1_4_or_later
    class GroupColumnWorker_WorkMode : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            if (!column.IsUniform())
            {
                GuiTools.PushColor(Metrics.GroupHeaderOpacityColor);
                DoMixedValuesIcon(rect);
                GuiTools.PopColor();
            } 
            else
            {
                var value = ((MechWorkModeDef)column.GetGroupValue())?.LabelCap;
                if (value != null)
                {
                    // todo: menu with work options to switch

                    GuiTools.PushFont(GameFont.Small);
                    GuiTools.PushTextAnchor(TextAnchor.MiddleCenter);
                    GuiTools.PushColor(Metrics.GroupHeaderOpacityColor);
                    rect.xMin += 3f;
                    Widgets.Label(rect, value);
                    GuiTools.PopColor();
                    GuiTools.PopTextAnchor();
                    GuiTools.PopFont();

                }
            }


        }

        public override bool CanSetValues()
        {
            return false;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return null;
        }

        public override object GetValue(Pawn pawn)
        {
            return pawn.GetMechControlGroup()?.WorkMode;
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {

        }
    }
#endif
}
