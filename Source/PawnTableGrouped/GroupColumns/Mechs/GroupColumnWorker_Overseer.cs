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
    class GroupColumnWorker_Overseer : GroupColumnWorker_Label
    {
        public override object GetValue(Pawn pawn)
        {
            return pawn.GetOverseer();
        }
    }
}
#endif