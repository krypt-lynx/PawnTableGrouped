using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class GroupWorker_ByGender : GroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn x, Pawn y)
            {
                return x.gender.Equals(x.gender);
            }

            public int GetHashCode(Pawn obj)
            {
                return obj.gender.GetHashCode();
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return Math.Sign((int)y.KeyPawn.gender - (int)x.KeyPawn.gender);
            }
        }

        public override IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_ByGender()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return keyPawn.gender.ToString().CapitalizeFirst() ?? "<unknown gender?>";
        }

        public override string MenuItemTitle()
        {
            return "by gender";
        }

        public override string Key()
        {
            return "gender";
        }
    }

}
