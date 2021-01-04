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
    public abstract class GroupWorker // todo: name
    {
        public abstract IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public abstract IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public abstract string TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn);

        public abstract string MenuItemTitle();
    }

    public class GroupWorker_ByColumn : GroupWorker
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
                return x.Title.CompareTo(y.Title);
            }
        }

        public Verse.WeakReference<GroupColumnWorker> resolver = null;

        public override IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_ByColumn(GroupColumnWorker resolver)
        {
            this.resolver = new Verse.WeakReference<GroupColumnWorker>(resolver);
            GroupingEqualityComparer = new ByColumnComparer(resolver.ColumnDef);
            GroupsSortingComparer = new ByColumnGroupComparer();
        }

        public override string TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            var value = resolver.Target?.GetValue(keyPawn);
            if (value == null)
            {
                return "";
            }
            else if (value is Texture2D tex)
            {
                return tex.name;
            } 
            else if (value is bool flag)
            {
                return flag ? "Yes".Translate() : "No".Translate();
            }
            else
            {
                return value.ToString();
            }
        }

        public override string MenuItemTitle()
        {
            return $"by column: {resolver.Target?.ColumnDef?.defName}";
        }
    }

    class GroupWorker_ByRace : GroupWorker
    {
        class ByRacePawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn x, Pawn y)
            {
                return x.kindDef.race.Equals(x.kindDef.race);
            }

            public int GetHashCode(Pawn obj)
            {
                return obj.kindDef.race.GetHashCode();
            }
        }

        class ByRaceGroupComparer : IComparer<PawnTableGroup>
        {
            public int Compare(PawnTableGroup x, PawnTableGroup y)
            {
                return Math.Sign(y.KeyPawn.kindDef.race.race.baseBodySize - x.KeyPawn.kindDef.race.race.baseBodySize);
            }
        }

        public override IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_ByRace()
        {
            GroupingEqualityComparer = new ByRacePawnComparer();
            GroupsSortingComparer = new ByRaceGroupComparer();
        }

        public override string TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
            return keyPawn.kindDef.race.label.CapitalizeFirst() ?? "<unknown race>";
        }

        public override string MenuItemTitle()
        {
            return "by race";
        }
    }

}
