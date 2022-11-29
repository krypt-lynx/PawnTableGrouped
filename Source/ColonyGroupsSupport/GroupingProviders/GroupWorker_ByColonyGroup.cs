using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using TacticalGroups;
using RimWorld;
using System.Collections;

namespace PawnTableGrouped
{
    class GroupWorker_ByColonyGroup : GroupWorker
    {
        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return 0;
            }
        }

        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_ByColonyGroup()
        {
            GroupsSortingComparer = new GroupComparer();
        }

        public override string Key()
        {
            return "colonygroups_group";
        }

        public override string MenuItemTitle()
        {
            return "colony groups";
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return "";
        }

        public override IEnumerable<PawnTableGroup> CreateGroups(PawnTable table, List<Pawn> pawns, Func<IEnumerable<Pawn>, IEnumerable<Pawn>> defaultPawnSort, List<GroupColumnWorker> columnResolvers)
        {
            if (!ColonyGroupsBridge.Instance.IsActive)
            {
                yield break;
            }

            HashSet<Pawn> tablePawns = new HashSet<Pawn>(pawns);
            HashSet<Pawn> ungrouped = new HashSet<Pawn>(pawns);

            foreach (var cgGroup in ColonyGroupsBridge.AllPawnGroups().Reverse<PawnGroup>())
            {
                ungrouped.ExceptWith(cgGroup.ActivePawns);

                var groupPawns = cgGroup.ActivePawns.Intersect(tablePawns).ToArray();
                if (groupPawns.Length > 0)
                {
                    yield return new PawnTableGroup(table, cgGroup.curGroupName ?? "", null, cgGroup.ActivePawns.Intersect(tablePawns), columnResolvers, () =>
                    {
                        return groupPawns.Length != cgGroup.ActivePawns.Count ? $"{groupPawns.Length} of {cgGroup.ActivePawns.Count}" : $"{groupPawns.Length}";
                    });
                }
            }

            if (ungrouped.Count > 0)
            {
                var pawnsPreservingOrder = pawns.Where(p => ungrouped.Contains(p));
                yield return new PawnTableGroup(table, "Ungrouped", null, defaultPawnSort(pawnsPreservingOrder), columnResolvers);
            }
        }


    }
}
