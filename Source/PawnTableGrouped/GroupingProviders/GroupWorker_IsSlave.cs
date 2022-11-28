using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
#if rw_1_3_or_later
    public class GroupWorker_IsSlave : SortingGroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn x, Pawn y)
            {
                return x.IsSlave == y.IsSlave;
            }

            public int GetHashCode(Pawn obj)
            {

                return obj.IsSlave.GetHashCode();
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup l, PawnTableGroup r)
            {
                var rSlave = r.KeyPawn.IsSlave ? 1 : 0;
                var lSlave = l.KeyPawn.IsSlave ? 1 : 0;
                return lSlave - rSlave;
            }
        }

        public GroupWorker_IsSlave()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return keyPawn.IsSlave ? "Slave" : "Colonist";
        }

        public override string MenuItemTitle()
        {
            return "is slave";
        }

        public override string Key()
        {
            return "slave";
        }
    }
#endif
}
