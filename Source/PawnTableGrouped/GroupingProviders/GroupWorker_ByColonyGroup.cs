#if rw_1_2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using TacticalGroups;
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

        public override IEnumerable<PawnTableGroup> CreateGroups(List<Pawn> pawns, Func<IEnumerable<Pawn>, IEnumerable<Pawn>> defaultPawnSort, List<GroupColumnWorker> columnResolvers)
        {
            if (!ColonyGroupsBridge.Instance.IsActive)
            {
                yield break;
            }

            HashSet<Pawn> ungrouped = new HashSet<Pawn>(pawns);

            foreach (var cgGroup in ColonyGroupsBridge.AllPawnGroups.Reverse<PawnGroup>())
            {
                ungrouped.ExceptWith(cgGroup.ActivePawns);
                yield return new PawnTableGroup(cgGroup.curGroupName ?? "", null, cgGroup.ActivePawns, columnResolvers);            
            }

            if (ungrouped.Count > 0)
            {
                var pawnsPreservingOrder = pawns.Where(p => ungrouped.Contains(p));
                yield return new PawnTableGroup("Ungrouped", null, pawnsPreservingOrder, columnResolvers);
            }
        }
    }
}
#endif
