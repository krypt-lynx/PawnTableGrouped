using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using WorkTab;

namespace PawnTableGrouped.WorkTab
{
    public class GroupColumnWorker_WorkGiver : GroupColumnWorker_WorkBase
    {
        WorkGiverDef Workgiver => (ColumnDef.Worker as PawnColumnWorker_WorkGiver)?.WorkGiver;

        public override object GetValue(Pawn pawn)
        {
            return pawn.GetPriority(Workgiver, MainTabWindow_WorkTab.VisibleHour);
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            pawn.SetPriority(Workgiver, (int)value, MainTabWindow_WorkTab.SelectedHours);
        }

        public override bool IsVisible(Pawn pawn)
        {
            return !pawn.WorkTypeIsDisabled(Workgiver.workType);
        }

        protected override Rect BoxRext(Rect cellRect)
        {
            float x = (int)(cellRect.x - 2 + (cellRect.width - 20f) / 2f);
            float y = (int)(cellRect.y + 6f);

            return new Rect(x, y, 20, 20);
        }

        protected override GameFont TextFont() => GameFont.Small;
        protected override GameFont SmallerTextFont() => GameFont.Small;
        protected override GameFont SmallestTextFont() => GameFont.Tiny;

        protected override int GetPrioritiesCount()
        {
            return global::WorkTab.Settings.maxPriority; // no, it this the *minimal* priority, not the maximal.
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

        protected override bool AlwaysShowPriority() => false;

    }
}
