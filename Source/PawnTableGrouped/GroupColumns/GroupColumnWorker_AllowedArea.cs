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
    class GroupColumnWorker_AllowedArea : GroupColumnWorker_Area
    {
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
            return pawn.playerSettings?.AreaRestriction;
        }

        public override bool IsVisible(Pawn pawn)
        {
            return pawn.Faction == Faction.OfPlayer;
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.AreaRestriction = (Area)value;
            }
        }
    }
}
