using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class GroupWorker_ByRace : SortingGroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn x, Pawn y)
            {
                return x.kindDef.race.Equals(y.kindDef.race);
            }

            public int GetHashCode(Pawn obj)
            {
                return obj.kindDef.race.GetHashCode();
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return Math.Sign(y.KeyPawn.kindDef.race.race.baseBodySize - x.KeyPawn.kindDef.race.race.baseBodySize);
            }
        }

        public GroupWorker_ByRace()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return keyPawn.kindDef.race.label.CapitalizeFirst() ?? "<unknown race>";
        }

        public override string MenuItemTitle()
        {
            return "by race";
        }

        public override string Key()
        {
            return "race";
        }
    }

}
