using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
#if rw_1_4_or_later
    internal class GroupWorker_ByXenotype : SortingGroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn l, Pawn r)
            {
                var lXenotype = l.genes?.Xenotype;
                var rXenotype = r.genes?.Xenotype;

                if (lXenotype == null)
                {
                    return rXenotype == null;
                } 
                else
                {
                    return lXenotype.Equals(rXenotype);
                }
            }

            public int GetHashCode(Pawn pawn)
            {
                return pawn.genes?.Xenotype?.GetHashCode() ?? 0;
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup l, PawnTableGroup r)
            {
                var lXenotype = l.KeyPawn.genes?.Xenotype?.displayPriority ?? -1000;
                var rXenotype = r.KeyPawn.genes?.Xenotype?.displayPriority ?? -1000;

                return Math.Sign(lXenotype - rXenotype);
            }
        }

        public GroupWorker_ByXenotype()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return keyPawn.genes?.Xenotype?.label.CapitalizeFirst() ?? "None";
        }

        public override string MenuItemTitle()
        {
            return "by xenotype";
        }

        public override string Key()
        {
            return "xenotype";
        }
    }
#endif
}
