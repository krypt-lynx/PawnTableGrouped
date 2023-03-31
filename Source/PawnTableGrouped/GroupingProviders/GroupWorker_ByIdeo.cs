using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
#if rw_1_3_or_later
    public class GroupWorker_ByIdeo : SortingGroupWorker
    {
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn l, Pawn r)
            {
                var lIdeo = l?.Ideo;
                var rIdeo = r?.Ideo;

                return rIdeo == lIdeo;
            }

            public int GetHashCode(Pawn obj)
            {
                return obj.Ideo?.GetHashCode() ?? 0;
            }

        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
            static Dictionary<Ideo, int> Order = new Dictionary<Ideo, int>();
            public static void UpdateOrder()
            {

                Order.Clear();

                int index = 0;
                foreach (var faction in Find.FactionManager.AllFactionsInViewOrder)
                {
                    var primaryIdeo = faction.ideos?.PrimaryIdeo;
                    if (primaryIdeo != null)
                    {
                        if (!Order.ContainsKey(primaryIdeo))
                        {
                            Order[primaryIdeo] = faction.IsPlayer ? (int.MinValue / 8) : index;
                        }
                    }
                    index++;
                }
            }

            public int Compare(PawnTableGroup l, PawnTableGroup r)
            {
                var lIdeo = l.KeyPawn?.Ideo;
                var rIdeo = r.KeyPawn?.Ideo;

                int lOrder;
                if (lIdeo == null || !Order.TryGetValue(lIdeo, out lOrder))
                {
                    lOrder = int.MaxValue / 8;
                }

                int rOrder;
                if (rIdeo == null || !Order.TryGetValue(rIdeo, out rOrder))
                {
                    rOrder = int.MaxValue / 8;
                }

                return Math.Sign(lOrder - rOrder);
            }
        }

        public GroupWorker_ByIdeo()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

        public override IComparer<PawnTableGroup> GroupsSortingComparer {
            get {
                GroupComparer.UpdateOrder();
               return base.GroupsSortingComparer;
            }
            protected set => base.GroupsSortingComparer = value;
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            var ideo = keyPawn.Ideo;

            if (ideo == null)
            {
                return "None";
            }

            if (ideo.Color == null)
            {
                return ideo.name;
            }
            else
            {
                return ideo.name.Colorize(ideo.Color);
            }
        }

        public override string MenuItemTitle()
        {
            return "by ideoligion";
        }

        public override string Key()
        {
            return "ideo";
        }
    }
#endif
}
