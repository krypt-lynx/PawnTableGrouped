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

namespace WildlifeTabAlt
{
    public class GroupColumnWorker_Checkbox : GroupColumnResolver
    {
        public bool GetValue(PawnColumnWorker_Checkbox worker, Pawn pawn)
        {
            return (bool)typeof(PawnColumnWorker_Checkbox).GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(worker, new object[] { pawn });
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => GetValue((PawnColumnWorker_Checkbox)ColumnDef.Worker, p));
        }            
    }
}
