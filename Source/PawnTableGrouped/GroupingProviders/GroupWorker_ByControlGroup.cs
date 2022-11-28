using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
#if rw_1_4_or_later
    internal class GroupWorker_ByControlGroup : GroupWorker
    {
        class GroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup l, PawnTableGroup r)
            {
                var lOverseerID = l.KeyPawn.GetOverseer()?.thingIDNumber ?? int.MaxValue;
                var rOverseerID = r.KeyPawn.GetOverseer()?.thingIDNumber ?? int.MaxValue;

                int result = Math.Sign(lOverseerID - rOverseerID);
                if (result == 0) {
                    var lGroupIndex = l.KeyPawn.GetMechControlGroup()?.Index ?? int.MaxValue;
                    var rGroupIndex = r.KeyPawn.GetMechControlGroup()?.Index ?? int.MaxValue;

                    result = Math.Sign(lGroupIndex - rGroupIndex);
                }
                return result;
            }
        }

        public GroupWorker_ByControlGroup()
        {
            GroupsSortingComparer = new GroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            var group = keyPawn.GetMechControlGroup();
           // var overseer = keyPawn.GetOverseer();
            if (group == null)
            {
                return "None";
            }

            return $"Group {group.Index} by {group?.Tracker?.Pawn}";
        }

        public override string MenuItemTitle()
        {
            return "by control group";
        }

        public override string Key()
        {
            return "control_group";
        }

        public override IEnumerable<PawnTableGroup> CreateGroups(PawnTableWrapper table, List<Pawn> pawns, Func<IEnumerable<Pawn>, IEnumerable<Pawn>> defaultPawnSort, List<GroupColumnWorker> columnResolvers)
        {
            Dictionary<MechanitorControlGroup, List<Pawn>> groups = new Dictionary<MechanitorControlGroup, List<Pawn>>();
            List<Pawn> nullGroup = new List<Pawn>();
            foreach (var pawn in pawns) 
            {
                var group = pawn.GetMechControlGroup();

                if (group == null)
                {
                    nullGroup.Add(pawn);
                }
                else
                {
                    if (!groups.TryGetValue(group, out var list))
                    {
                        list = new List<Pawn>();
                        groups[group] = list;
                    }
                    list.Add(pawn);
                }
            }

            foreach (var group in groups.Values)
            {
                var aPawn = group[0];
                yield return new PawnTableGroup(table, TitleForGroup(group, aPawn), aPawn, group, columnResolvers);
            }     
            if (nullGroup.Count > 0)
            {
                var aPawn = nullGroup[0];
                yield return new PawnTableGroup(table, TitleForGroup(nullGroup, aPawn), aPawn, nullGroup, columnResolvers);
            }
        }
    }
#endif
}
