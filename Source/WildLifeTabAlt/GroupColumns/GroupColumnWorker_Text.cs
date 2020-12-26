using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt.GroupColumns
{
    class GroupColumnWorker_Text : GroupColumnResolver
    {
        public string GetTextFor(PawnColumnWorker_Text worker, Pawn pawn)
        {
            return (string)typeof(PawnColumnWorker_Text).GetMethod("GetTextFor", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(worker, new object[] { pawn });
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => GetTextFor((PawnColumnWorker_Text)ColumnDef.Worker, p));
        }
    }
}
