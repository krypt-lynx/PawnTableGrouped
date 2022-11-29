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

namespace PawnTableGrouped
{
	public class GCW_WorkPriority_Config
	{
		public int prioritiesCount = 4;
		public int defaultPriority = 3;
		public string priorityColorMethod = "RimWorld.WidgetsWork:ColorOfPriority";

		[Unsaved(false)]
		Func<int, Color> getColor;
		[Unsaved(false)]
		Color getColorMethodFailsafeColor = new Color(0.74f, 0.74f, 0.74f);

		public Color ColorOfPriority(int priority)
		{
			if (getColor == null)
            {
				var getColorMethod = HarmonyLib.AccessTools.Method(priorityColorMethod);
				try
				{
					if (getColorMethod != null)
					{
						getColor = Dynamic.StaticRetMethod<int, Color>(getColorMethod);
					}
				}
				catch { }

				getColor ??= (priority) => getColorMethodFailsafeColor;
			}

			return getColor(priority);
		}
	}

	public class GroupColumnWorker_WorkPriority : GroupColumnWorker_WorkBase
    { 
        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return GetWorkerConfig<GCW_WorkPriority_Config>().defaultPriority;
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

        protected override Rect BoxRext(Rect cellRect)
        {
            float x = (int)(cellRect.x + (cellRect.width - 25f) / 2f);
            float y = (int)(cellRect.y + 3.5f);

            return new Rect(x, y, 25, 25);
        }

        protected override GameFont TextFont() => GameFont.Medium;
        protected override GameFont SmallerTextFont() => GameFont.Small;
        protected override GameFont SmallestTextFont() => GameFont.Tiny;

        protected override int GetPrioritiesCount() => 5;

        protected override bool PlaySounds() => true;

        protected override bool UseMouseScroll() => false;

        protected override bool AlwaysShowPriority() => true;
    }
}
