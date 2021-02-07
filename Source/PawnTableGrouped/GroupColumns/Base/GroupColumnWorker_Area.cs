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
    public abstract class GroupColumnWorker_Area : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
            KWidgets.DoAllowedAreaSelectors(rect, column, GetAreaLabel);
            GuiTools.PopColor();

            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }

        public virtual string GetAreaLabel(Area area)
        {
            return AreaUtility.AreaAllowedLabel_Area(area);
        }

        public override bool CanSetValues()
        {
            return true;
        }
            
        public override string GetStringValue(Pawn pawn)
        {
            return ((Area)GetValue(pawn))?.Label ?? "Unrestricted";
        }
    }
}
