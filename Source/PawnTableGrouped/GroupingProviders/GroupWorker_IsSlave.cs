using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    class GroupWorker_IsSlave : GroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn x, Pawn y)
            {
                return SimpleSlaveryWrapper.IsPawnColonySlave(x) == SimpleSlaveryWrapper.IsPawnColonySlave(y);
            }

            public int GetHashCode(Pawn obj)
            {
                return 0;
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return Math.Sign(Convert.ToInt32(SimpleSlaveryWrapper.IsPawnColonySlave(x.KeyPawn)) - Convert.ToInt32(SimpleSlaveryWrapper.IsPawnColonySlave(y.KeyPawn)));
            }
        }

        public override IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_IsSlave()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return SimpleSlaveryWrapper.IsPawnColonySlave(keyPawn) ? "Slave" : "Colonist";
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
}
