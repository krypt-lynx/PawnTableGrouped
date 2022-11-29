using RimWorld;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PawnTableGrouped.WorkTab
{
	public class GroupColumnWorker_WorkPriority : GroupColumnWorker_WorkBase
	{

        protected override Rect BoxRext(Rect cellRect)
        {
            float x = (int)(cellRect.x + (cellRect.width - 25f) / 2f);
            float y = (int)(cellRect.y + 3.5f);

            return new Rect(x, y, 25, 25);
        }

        public override object GetValue(Pawn pawn)
        {
            return pawn.workSettings.GetPriority(ColumnDef.workType);
        }

        public override bool IsVisible(Pawn pawn)
        {
            return !pawn.WorkTypeIsDisabled(ColumnDef.workType);
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            var priority = (int)value;

            if (!pawn.WorkTypeIsDisabled(ColumnDef.workType))
            {
                pawn.workSettings.SetPriority(ColumnDef.workType, priority);
            }
        }

        protected override GameFont TextFont() => GameFont.Medium;
        protected override GameFont SmallerTextFont() => GameFont.Small;
        protected override GameFont SmallestTextFont() => GameFont.Tiny;

        protected override int GetPrioritiesCount()
        {
            return global::WorkTab.Settings.maxPriority + 1;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return global::WorkTab.Settings.defaultPriority;
        }

        protected override bool PlaySounds()
        {
            return global::WorkTab.Settings.playSounds;
        }

        protected override bool UseMouseScroll()
        {
            return !global::WorkTab.Settings.disableScrollwheel;
        }

        protected override bool AlwaysShowPriority() => true;
    }
}
