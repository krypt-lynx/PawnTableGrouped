using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class GroupColumnWorker_Gap : GroupColumnWorker
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
            return "Oops";
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return true;
        }

        public override bool IsGroupVisible(IEnumerable<Pawn> pawns)
        {
            return false;
        }

        public override bool IsVisible(Pawn pawn)
        {
            return false;
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {

        }

        public override void DoCell(Rect cellRect, PawnTableGroupColumn column, PawnTable table)
        {

        }
    }
}
