using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using PawnTableGrouped;

namespace PawnTableGrouped
{
	public abstract class GroupColumnWorker
	{


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
