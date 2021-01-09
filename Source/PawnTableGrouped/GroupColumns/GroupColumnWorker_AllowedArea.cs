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
    class GroupColumnWorker_AllowedArea : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
            KWidgets.DoAllowedAreaSelectors(rect, column);
            GuiTools.PopColor();

            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return Find.CurrentMap.areaManager.Home;
        }

        public override object GetValue(Pawn pawn)
        {
            if (pawn.Faction != Faction.OfPlayer)
            {
                return null;
            }
            /*if (Find.CurrentMap == null)
            {
                return;
            }*/
            return pawn.playerSettings?.AreaRestriction;

        }

        public override bool IsVisible(Pawn pawn)
        {
            return pawn.Faction == Faction.OfPlayer;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.AreaRestriction = (Area)value;
            }
        }

        public override string GetStringValue(Pawn pawn)
        {
            return ((Area)GetValue(pawn))?.Label ?? "Unrestricted";
        }
    }
}
