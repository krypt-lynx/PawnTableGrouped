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
            return "Oops";
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return true;
        }

        public override bool IsGroupVisible(IEnumerable<Pawn> pawns)
        {
            return Mod.Settings.showDummyColumns;
        }

        public override bool IsVisible(Pawn pawn)
        {
            return false;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            
        }

        public override void DoCell(Rect cellRect, PawnTableGroupColumn column, PawnTable table)
        {
            if (Mod.Settings.showDummyColumns)
            {
                GuiTools.PushColor(Color.yellow);
                GuiTools.Box(cellRect.ContractedBy(1), EdgeInsets.One);
                GuiTools.PopColor();
                
                TooltipHandler.TipRegion(cellRect, $"{ColumnDef.defName}: {ColumnDef.workerClass.FullName}");
            }   
        }

        public override bool IsDummy()
        {
            return true;
        }
    }
}
