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
	public abstract class GroupColumnResolver
    {

		public static GroupColumnResolverDef GetResolverSilentFail(PawnColumnDef column)
        {
			var resolverDef = DefDatabase<GroupColumnResolverDef>.GetNamedSilentFail(column.defName);
			if (resolverDef == null)
            {
				if (column.Worker is PawnColumnWorker_Checkbox)
                {
                    resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Checkbox));
                }
                else if (column.Worker is PawnColumnWorker_Trainable)
                {
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Trainable));
				}
				else if (column.Worker is PawnColumnWorker_Icon)
				{
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Icon));
				}
				else if (column.Worker is PawnColumnWorker_Text)
                {
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Text));
				}
				else if (column.Worker is PawnColumnWorker_MedicalCare)
                {
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_MedicalCare));
                }

			}
			return resolverDef;
		}

        private static GroupColumnResolverDef CreateGroupColumnDef(PawnColumnDef column, Type workerType)
        {
            GroupColumnResolverDef resolverDef = new GroupColumnResolverDef
            {
                defName = column.defName,
                workerClass = workerType,
                modContentPack = WildLifeTabMod.Content
            };
            DefGenerator.AddImpliedDef(resolverDef);
            return resolverDef;
        }

        public GroupColumnResolverDef def;

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



		public virtual void DoCell(Rect cellRect, PawnTableGroup group, PawnTable table)
		{
			if (group.Pawns.Count > 0)
			{
				if (IsUniform(group.Pawns))
				{
					ColumnDef.Worker.DoCell(cellRect, group.Pawns.First(), table);
				}
				else
                {
					GuiTools.PushColor(Color.white);
					GuiTools.PushTextAnchor(TextAnchor.MiddleCenter);
					GuiTools.PushFont(GameFont.Small);
					Widgets.Label(cellRect, "...");
					GuiTools.PopFont();
					GuiTools.PopTextAnchor();
					GuiTools.PopColor();
                }
			}
        }
    }

	public class GroupColumnResolverDef : Def
    {
		// Token: 0x0400200D RID: 8205
		[Unsaved(false)]
		private GroupColumnResolver workerInt;

		public Type workerClass = typeof(GroupColumnResolver);

		public GroupColumnResolver Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (GroupColumnResolver)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}
	}
}
