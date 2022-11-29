using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class Grouping<K, T> : IGrouping<K, T>
    {
        IEnumerable<T> Enumerable = null; 

        public K Key { get; }

        public Grouping(K key, IEnumerable<T> enumerable)
        {
            Key = key;
            Enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }
    }

    public abstract class GroupWorker
    {
        public virtual IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public abstract TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn);
        public abstract string MenuItemTitle();
        public abstract string Key();

        public abstract IEnumerable<PawnTableGroup> CreateGroups(PawnTable table, List<Pawn> pawns, Func<IEnumerable<Pawn>, IEnumerable<Pawn>> defaultPawnSort, List<GroupColumnWorker> columnResolvers);

        public TaggedString TitleForGroup(IGrouping<Pawn, Pawn> pawns)
        {
            return TitleForGroup(pawns, pawns.Key);
        }
    }

    public abstract class SortingGroupWorker : GroupWorker // todo: name
    {
        public virtual IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }

        public override IEnumerable<PawnTableGroup> CreateGroups(PawnTable table, List<Pawn> pawns, Func<IEnumerable<Pawn>, IEnumerable<Pawn>> defaultPawnSort, List<GroupColumnWorker> columnResolvers)
        {
            var groups = pawns.GroupBy(p => p, GroupingEqualityComparer);
            foreach (var group in groups)
            {
                yield return new PawnTableGroup(table, TitleForGroup(group), group.Key, defaultPawnSort(group), columnResolvers);
            }
        }
    }
}
