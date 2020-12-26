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
    class GroupColumnWorker_Icon : GroupColumnResolver
    {
        public Texture2D GetIconFor(PawnColumnWorker_Icon worker, Pawn pawn)
        {
            return (Texture2D)typeof(PawnColumnWorker_Icon).GetMethod("GetIconFor", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(worker, new object[] { pawn });
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => GetIconFor((PawnColumnWorker_Icon)ColumnDef.Worker, p));
        }
    }
}
