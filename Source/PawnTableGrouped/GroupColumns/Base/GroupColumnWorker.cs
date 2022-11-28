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

		private object defaultConfig = null;

		public T GetWorkerConfig<T>() where T : class, new()
		{
				return (def.workerConfig as T) ?? (T)defaultConfig ?? (T)(defaultConfig = new T());
		}

		private bool needCreateGroupWorker = true;
		public GroupWorker_ByColumn groupWorker = null;
		public GroupWorker_ByColumn GroupWorker
        {
			get
            {
				if (needCreateGroupWorker && groupWorker == null)
                {
					needCreateGroupWorker = false;
					if (ColumnDef.sortable && !IsDummy())
					{
						groupWorker = new GroupWorker_ByColumn(this);
					}
                }
				return groupWorker;
            }
        }

		private PawnColumnDef columnDef = null;
		public PawnColumnDef ColumnDef => columnDef ??= DefDatabase<PawnColumnDef>.GetNamed(def.defName);

		abstract public bool CanSetValues();
		abstract public object DefaultValue(IEnumerable<Pawn> pawns);
		abstract public object GetValue(Pawn pawn);
		abstract public void SetValue(Pawn pawn, object value, PawnTable table);

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

		public virtual void SetGroupValue(IEnumerable<Pawn> pawns, object value, PawnTable table)
		{
			foreach (var pawn in pawns)
			{
				if (IsVisible(pawn))
				{
					SetValue(pawn, value, table);
				}
			}
		}

		public virtual void CopyToGroup(Pawn pawn, IEnumerable<Pawn> pawns, PawnTable table)
        {
			var value = GetValue(pawn);
			SetGroupValue(pawns, value, table);
        }

		public virtual bool IsVisible(Pawn pawn)
        {
			return true;
        }

		public virtual bool IsGroupVisible(IEnumerable<Pawn> pawns)
        {
			return pawns.Any(p => IsVisible(p));
        }

		public virtual string GetStringValue(Pawn pawn)
		{
			return GetValue(pawn).ToString();
		}

		public virtual void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
		{
			
        }

		protected virtual void DoMixedValuesIcon(Rect rect)
		{
			GuiTools.PushTextAnchor(TextAnchor.MiddleCenter);
			GuiTools.PushFont(GameFont.Small);
			Widgets.Label(rect, "...");
			GuiTools.PopFont();
			GuiTools.PopTextAnchor();
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

		public virtual bool IsDummy()
        {
			return false;
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
