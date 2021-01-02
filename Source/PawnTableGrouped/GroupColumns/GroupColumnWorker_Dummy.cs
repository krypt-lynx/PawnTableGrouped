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
    public class GroupColumnWorker_Dummy : GroupColumnWorker
    {
        public override bool CanSetValues()
        {
            return false;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return null;
        }

        public override object GetValue(Pawn pawn)
        {
            return null;
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return true;
        }

        public override bool IsVisible(IEnumerable<Pawn> pawns)
        {
            return false;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            
        }

        public override void DoCell(Rect cellRect, PawnTableGroup group, PawnTable table, int columnIndex)
        {
            
        }
    }
}
