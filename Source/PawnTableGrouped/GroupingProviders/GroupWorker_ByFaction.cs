using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    class GroupWorker_ByFaction : GroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            IEqualityComparer<Faction> innerComparer = EqualityComparer<Faction>.Default;
            public bool Equals(Pawn x, Pawn y)
            {
                var xf = x.FactionOrExtraMiniOrHomeFaction;
                var yf = y.FactionOrExtraMiniOrHomeFaction;

                return innerComparer.Equals(xf, yf);
            }

            public int GetHashCode(Pawn obj)
            {
                return innerComparer.GetHashCode(obj.FactionOrExtraMiniOrHomeFaction);
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                var xf = x.KeyPawn.FactionOrExtraMiniOrHomeFaction;
                var yf = y.KeyPawn.FactionOrExtraMiniOrHomeFaction;

                var xp = xf?.IsPlayer ?? false;
                var yp = yf?.IsPlayer ?? false;

                var xo = xf?.def.listOrderPriority ?? 0;
                var yo = yf?.def.listOrderPriority ?? 0;

                if (xp != yp)
                {
                    return Math.Sign(Convert.ToInt32(yp) - Convert.ToInt32(xp));
                }
                else
                {
                    return Math.Sign(yo - xo);
                }

            }
        }

        public override IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_ByFaction()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override string TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return keyPawn.FactionOrExtraMiniOrHomeFaction?.Name ?? "";
        }

        public override string MenuItemTitle()
        {
            return "by faction";
        }

        public override string Key()
        {
            return "faction";
        }
    }
}
