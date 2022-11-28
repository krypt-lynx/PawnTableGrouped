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
    class GroupColumnWorker_Label : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            if (column.IsUniform())
            {
                var pawn = GetRepresentingPawn(column.Group.Pawns);

                GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
                ColumnDef.Worker.DoCell(rect, pawn, table);
                GuiTools.PopColor();    
            }
        }

        public override bool CanSetValues() => false;

        public override object DefaultValue(IEnumerable<Pawn> pawns) => null;

        public override void SetValue(Pawn pawn, object value, PawnTable table) { }

        public override object GetValue(Pawn pawn)
        {
            return pawn;
        }
    }
}
