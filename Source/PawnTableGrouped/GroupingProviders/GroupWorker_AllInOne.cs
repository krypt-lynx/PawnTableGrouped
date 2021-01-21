using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class GroupWorker_AllInOne : GroupWorker
    {
        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return 0;
            }
        }

        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_AllInOne()
        {
            GroupsSortingComparer = new GroupComparer();
        }

        public override IEnumerable<IGrouping<Pawn, Pawn>> CreateGroups(List<Pawn> pawns)
        {
            if (pawns.Count > 0) {
                yield return new Grouping<Pawn, Pawn>(pawns.First(), pawns);
            }
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return "All";
        }

        public override string MenuItemTitle()
        {
            return "all in one";
        }

        public override string Key()
        {
            return "all";
        }

    }

}
