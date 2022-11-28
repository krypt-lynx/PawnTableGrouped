using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

#if rw_1_4_or_later
namespace PawnTableGrouped
{
    class GroupColumnWorker_AutoRepair : GroupColumnWorker_CheckboxBase
    {
        public override object DefaultValue(IEnumerable<Pawn> pawns) => false;

        public override object GetValue(Pawn pawn)
        {
            CompMechRepairable comp = pawn.GetComp<CompMechRepairable>();
            return comp?.autoRepair ?? false;
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            if (CanSetValue(pawn))
            {
                CompMechRepairable comp = pawn.GetComp<CompMechRepairable>();
                comp.autoRepair = (bool)value;
            }
        }

        public override bool IsVisible(Pawn pawn) => CanSetValue(pawn);

        public override bool CanSetValue(Pawn pawn)
        {
            if (pawn.Faction == Faction.OfPlayer && pawn.RaceProps.IsMechanoid && pawn.GetOverseer() == null)
            {
                return false;
            }
            CompMechRepairable comp = pawn.GetComp<CompMechRepairable>();
            if (comp == null)
            {
                return false;
            }
            return true;
        }

    }
}
#endif