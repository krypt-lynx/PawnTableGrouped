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
        public abstract string Key();
    }




}
