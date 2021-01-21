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
        public abstract IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public abstract TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn);
        public abstract string MenuItemTitle();
        public abstract string Key();

        public abstract IEnumerable<IGrouping<Pawn, Pawn>> CreateGroups(List<Pawn> pawns);
    }

    public abstract class SortingGroupWorker : GroupWorker // todo: name
    {
        public abstract IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }

        public override IEnumerable<IGrouping<Pawn, Pawn>> CreateGroups(List<Pawn> pawns)
        {
           return pawns.GroupBy(p => p, GroupingEqualityComparer);
        }
    }




}
