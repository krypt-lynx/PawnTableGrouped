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
	public class ListGroupHeaderMapping
    {
		public string columnWorkerType;
		public string groupWorkerType;
		public bool isInteractive;
	}

	public class ClassMappingDef : Def
	{
		public List<ListGroupHeaderMapping> mapping;


		[Unsaved(false)]
		private Dictionary<Type, Type> mapping_ = null;

		public Dictionary<Type, Type> Mapping
		{
			get
			{
				if (mapping_ == null)
				{
					mapping_ = new Dictionary<Type, Type>();

					foreach (var def in mapping)
					{
						// reading types manually to suppress parser error if supported mod is not present
						var columnWorker = GenTypes.GetTypeInAnyAssembly(def.columnWorkerType);
						var groupWorker = GenTypes.GetTypeInAnyAssembly(def.groupWorkerType);
						if (columnWorker != null && groupWorker != null)
						{
							mapping_[columnWorker] = groupWorker;
						}
					}

					$"Loaded {mapping_.Count} of {mapping.Count} table column mappings".Log();
				}
				return mapping_;
			}
		}
	}

	public abstract class GroupColumnWorker
	{
	

			


		public static GroupColumnWorkerDef GetResolverSilentFail(PawnColumnDef column)
        {
			var resolverDef = DefDatabase<GroupColumnWorkerDef>.GetNamedSilentFail(column.defName);
			if (resolverDef == null)
            {
				var mapping = DefDatabase<ClassMappingDef>.GetNamed("GroupHeadersMapping");
				var workerClass = column.workerClass;
				Type headerType = null;

				while (workerClass != null)
                {
					if (mapping.Mapping.TryGetValue(workerClass, out headerType) && headerType != null)
					{
						resolverDef = CreateGroupColumnDef(column, headerType);
					}

					if (resolverDef != null)
                    {
						break;
                    }
					workerClass = workerClass.BaseType;
				}

				if (resolverDef == null)
                {
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Dummy));
                }

				$"Header for column {column.defName}: {resolverDef.workerClass.FullName}".Log();
			}


			return resolverDef;
		}

        private static GroupColumnWorkerDef CreateGroupColumnDef(PawnColumnDef column, Type workerType)
        {
            GroupColumnWorkerDef resolverDef = new GroupColumnWorkerDef
            {
                defName = column.defName,
                workerClass = workerType,
                modContentPack = Mod.Instance.Content
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
