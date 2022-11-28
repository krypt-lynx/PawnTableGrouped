using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

#if rw_1_4_or_later
namespace PawnTableGrouped
{
    class GroupColumnWorker_DraftMech : GroupColumnWorker_CheckboxBase
    {
        public override object DefaultValue(IEnumerable<Pawn> pawns) => false;

        public override object GetValue(Pawn pawn)
        {
            return pawn.Drafted;
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            if (CanSetValue(pawn))
            {
                var drafted = (bool)value;
                if (drafted != pawn.Drafted)
                {
                    pawn.drafter.Drafted = drafted;
                }
            }
        }

        public override bool IsVisible(Pawn pawn) => MechanitorUtility.CanDraftMech(pawn);

        public override bool CanSetValue(Pawn pawn) => MechanitorUtility.CanDraftMech(pawn);
    }
}
#endif