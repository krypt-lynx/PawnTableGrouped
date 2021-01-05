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

		abstract public bool CanSetValues();
		abstract public object DefaultValue(IEnumerable<Pawn> pawns);
		abstract public object GetValue(Pawn pawn);
		abstract public void SetValue(Pawn pawn, object value);

		public virtual Pawn GetRepresentingPawn(IEnumerable<Pawn> pawns)
        {
			return pawns.FirstOrDefault(p => IsVisible(p));
        }

		public virtual bool IsUniform(IEnumerable<Pawn> pawns)
		{
			return pawns.Where(p => IsVisible(p)).IsUniform(p => GetValue(p));
		}

		public virtual object GetGroupValue(IEnumerable<Pawn> pawns)
		{
			var pawn = pawns.FirstOrDefault(p => IsVisible(p));
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
				if (IsVisible(pawn))
				{
					SetValue(pawn, value);
				}
			}
		}

		public virtual void CopyToGroup(Pawn pawn, PawnTableGroupColumn column)
        {
			var value = GetValue(pawn);
			column.SetGroupValue(value);
        }

		public virtual bool IsVisible(Pawn pawn)
        {
			return true;
        }

		public virtual bool IsGroupVisible(IEnumerable<Pawn> pawns)
        {
			return pawns.Any(p => IsVisible(p));
        }

		public virtual void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
		{
			
        }

		static Color mixedTextColor = new Color(1, 1, 1, 0.6f);

		protected virtual void DoMixedValuesIcon(Rect rect)
		{
			GuiTools.PushColor(mixedTextColor);
			GuiTools.PushTextAnchor(TextAnchor.MiddleCenter);
			GuiTools.PushFont(GameFont.Small);
			Widgets.Label(rect, "...");
			GuiTools.PopFont();
			GuiTools.PopTextAnchor();
			GuiTools.PopColor();
		}

		protected virtual void DoMixedValuesWidget(Rect rect, PawnTableGroupColumn column)
        {
			DoMixedValuesIcon(rect);
			if (Widgets.ButtonInvisible(rect, false))
			{
				column.SetGroupValue(column.GetDefaultValue());
			}
			if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
			{
				Event.current.Use();
			}
		}
	}

	public class GroupColumnWorkerDef : Def
    {
		[Unsaved(false)]
		private GroupColumnWorker workerInt;

		public Type workerClass = typeof(GroupColumnWorker);
		public object workerConfig = null;

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
