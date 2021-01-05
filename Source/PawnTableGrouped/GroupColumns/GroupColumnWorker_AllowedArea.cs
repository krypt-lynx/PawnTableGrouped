using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped.GroupColumns
{
    class GroupColumnWorker_AllowedArea : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            if (!column.IsUniform())
            {
                DoMixedValuesWidget(rect, column);
            }
            else
            {
                var pawn = GetRepresentingPawn(column.Group.Pawns);
                ColumnDef.Worker.DoCell(rect, pawn, table);
                CopyToGroup(pawn, column.Group.Pawns);
            }
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return Find.CurrentMap.areaManager.Home;
        }

        public override object GetValue(Pawn pawn)
        {
            if (pawn.Faction != Faction.OfPlayer)
            {
                return null;
            }
            /*if (Find.CurrentMap == null)
            {
                return;
            }*/
            return pawn.playerSettings?.AreaRestriction;

        }

        public override void SetValue(Pawn pawn, object value)
        {
            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.AreaRestriction = (Area)value;
            }
        }
    }
}
