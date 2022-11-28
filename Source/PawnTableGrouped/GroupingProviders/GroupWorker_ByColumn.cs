using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class GroupWorker_ByColumn : SortingGroupWorker
    {
        class ByColumnComparer : IEqualityComparer<Pawn>
        {
            public PawnColumnDef PawnColumnDef { get; private set; }

            public ByColumnComparer(PawnColumnDef pawnColumnDef)
            {
                this.PawnColumnDef = pawnColumnDef;
            }

            public bool Equals(Pawn x, Pawn y)
            {
                return PawnColumnDef.Worker.Compare(x, y) == 0;
            }

            public int GetHashCode(Pawn obj)
            {
                return 0;
            }
        }

        class ByColumnGroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return x.Title.RawText.CompareTo(y.Title.RawText);
            }
        }

        public Verse.WeakReference<GroupColumnWorker> resolver = null;

        public GroupWorker_ByColumn(GroupColumnWorker resolver)
        {
            this.resolver = new Verse.WeakReference<GroupColumnWorker>(resolver);
            GroupingEqualityComparer = new ByColumnComparer(resolver.ColumnDef);
            GroupsSortingComparer = new ByColumnGroupComparer();
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return resolver.Target?.GetStringValue(keyPawn) ?? "";
        }

        public override string MenuItemTitle()
        {
            return $"{resolver.Target?.ColumnDef?.defName}";
        }

        public override string Key()
        {
            return $"column_{resolver?.Target?.ColumnDef.defName ?? ""}";
        }
    }

}
