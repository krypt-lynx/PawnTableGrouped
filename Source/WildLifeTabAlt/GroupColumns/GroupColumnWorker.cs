using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using WildlifeTabAlt.GroupColumns;

namespace WildlifeTabAlt
{
	public abstract class GroupColumnWorker
    {

		public static GroupColumnWorkerDef GetResolverSilentFail(PawnColumnDef column)
        {
			var resolverDef = DefDatabase<GroupColumnWorkerDef>.GetNamedSilentFail(column.defName);
			if (resolverDef == null)
            {
				if (Mod.Settings.interactiveGroupHeaderExperimental)
				{
					if (column.Worker is PawnColumnWorker_Checkbox)
					{
						resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Checkbox));
					}
					else if (column.Worker is PawnColumnWorker_Trainable)
					{
						resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Trainable));
					}
					else if (column.Worker is PawnColumnWorker_MedicalCare)
					{
						resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_MedicalCare));
					}
				}

				if (column.Worker is PawnColumnWorker_Icon)
				{
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Icon));
				}
				else if (column.Worker is PawnColumnWorker_Text)
				{
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Text));
				}


			}
			return resolverDef;
		}

        private static GroupColumnWorkerDef CreateGroupColumnDef(PawnColumnDef column, Type workerType)
        {
            GroupColumnWorkerDef resolverDef = new GroupColumnWorkerDef
            {
                defName = column.defName,
                workerClass = workerType,
                modContentPack = Mod.Content
            };
            DefGenerator.AddImpliedDef(resolverDef);
            return resolverDef;
        }

        public GroupColumnWorkerDef def;

		private PawnColumnDef columnDef;
		public PawnColumnDef ColumnDef
        {
			get
            {
				if (columnDef == null)
                {
					columnDef = DefDatabase<PawnColumnDef>.GetNamed(def.defName);
                }
				return columnDef;
            }
        }



		abstract public bool IsUniform(IEnumerable<Pawn> pawns);

		public virtual object GetGroupValue(IEnumerable<Pawn> pawns)
		{
			var pawn = pawns.FirstOrDefault();
			return pawn != null ? GetValue(pawn) : null;
		}

		public virtual void SetGroupValue(IEnumerable<Pawn> pawns, object value)
		{
			if (value == null)
			{
				return;
			}

			foreach (var pawn in pawns)
			{
				SetValue(pawn, value);
			}
		}

		abstract public bool CanSetValues();
		abstract public object DefaultValue();
		abstract public object GetValue(Pawn pawn);
		abstract public void SetValue(Pawn pawn, object value);
		abstract public bool IsVisible(IEnumerable<Pawn> pawns);

		static Color mixedTextColor = new Color(1, 1, 1, 0.6f);

		public virtual void DoCell(Rect cellRect, PawnTableGroup group, PawnTable table, int columnIndex)
		{

        }

		protected virtual void DoMixedValuesIcon(Rect cellRect)
		{
			GuiTools.PushColor(mixedTextColor);
			GuiTools.PushTextAnchor(TextAnchor.MiddleCenter);
			GuiTools.PushFont(GameFont.Small);
			Widgets.Label(cellRect, "...");
			GuiTools.PopFont();
			GuiTools.PopTextAnchor();
			GuiTools.PopColor();
		}

	}

	public class GroupColumnWorkerDef : Def
    {
		// Token: 0x0400200D RID: 8205
		[Unsaved(false)]
		private GroupColumnWorker workerInt;

		public Type workerClass = typeof(GroupColumnWorker);

		public GroupColumnWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (GroupColumnWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}
	}
}
