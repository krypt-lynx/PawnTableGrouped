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
            return new AreaData(Find.CurrentMap.areaManager.Home);
        }

        public override object GetValue(Pawn pawn)
        {
            if (pawn.Faction != Faction.OfPlayer)
            {
                return AreaData.Undefined;
            }

#if rw_1_2_or_earlier
            return new AreaData(pawn.playerSettings?.AreaRestriction);
#else

            if (pawn.playerSettings.SupportsAllowedAreas)
            {
                return new AreaData(pawn.playerSettings?.AreaRestriction);
            }
            if (AnimalPenUtility.NeedsToBeManagedByRope(pawn))
            {
                return new AreaData(AnimalPenUtility.GetCurrentPenOf(pawn, false));
            }

            return AreaData.Undefined;
#endif
        }

        public override bool IsVisible(Pawn pawn)
        {
            return pawn.Faction == Faction.OfPlayer;
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            $"GroupColumnWorker_AllowedArea.SetValue({pawn},{value},{table}) ".Log();

            if (pawn.playerSettings != null)
            {
#if rw_1_2_or_earlier
                pawn.playerSettings.AreaRestriction = (Area)value;
#else
                if (pawn.playerSettings.SupportsAllowedAreas)
                {
                    var data = (AreaData)value;
                    if (data.Type == AreaData.DataType.Area)
                    {
                        pawn.playerSettings.AreaRestriction = data.Area;
                    }
                }
#endif
            }
        }
    }
}
