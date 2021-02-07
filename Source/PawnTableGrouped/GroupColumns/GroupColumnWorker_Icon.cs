using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
	public class GroupColumnWorker_Icon : GroupColumnWorker
    {
		static Func<PawnColumnWorker_Icon, Pawn, Texture2D> getIconFor = FastAccess.InstanceRetMethod<PawnColumnWorker_Icon, Pawn, Texture2D>(
			typeof(PawnColumnWorker_Icon).GetMethod("GetIconFor", BindingFlags.NonPublic | BindingFlags.Instance));
		static Func<PawnColumnWorker_Icon, Pawn, Vector2> setIconSize = FastAccess.InstanceRetMethod<PawnColumnWorker_Icon, Pawn, Vector2>(
			typeof(PawnColumnWorker_Icon).GetMethod("GetIconSize", BindingFlags.NonPublic | BindingFlags.Instance));
		static Func<PawnColumnWorker_Icon, Pawn, Color> getIconColor = FastAccess.InstanceRetMethod<PawnColumnWorker_Icon, Pawn, Color>(
			typeof(PawnColumnWorker_Icon).GetMethod("GetIconColor", BindingFlags.NonPublic | BindingFlags.Instance));
		static Func<PawnColumnWorker_Icon, Pawn, string> getIconTip = FastAccess.InstanceRetMethod<PawnColumnWorker_Icon, Pawn, string>(
			typeof(PawnColumnWorker_Icon).GetMethod("GetIconTip", BindingFlags.NonPublic | BindingFlags.Instance));

		static Action<PawnColumnWorker_Icon, Pawn> clickedIcon = FastAccess.InstanceVoidMethod<PawnColumnWorker_Icon, Pawn>(
			typeof(PawnColumnWorker_Icon).GetMethod("ClickedIcon", BindingFlags.NonPublic | BindingFlags.Instance));
		static Action<PawnColumnWorker_Icon, Pawn> paintedIcon = FastAccess.InstanceVoidMethod<PawnColumnWorker_Icon, Pawn>(
			typeof(PawnColumnWorker_Icon).GetMethod("PaintedIcon", BindingFlags.NonPublic | BindingFlags.Instance));
		static Func<PawnColumnWorker_Icon, int> padding = FastAccess.InstanceRetMethod<PawnColumnWorker_Icon, int>(
			typeof(PawnColumnWorker_Icon).GetMethod("get_Padding", BindingFlags.NonPublic | BindingFlags.Instance));


		public Texture2D GetIconFor(Pawn pawn)
        {
			return getIconFor((PawnColumnWorker_Icon)ColumnDef.Worker, pawn);
        }

		public Vector2 GetIconSize(Pawn pawn)
		{
			return setIconSize((PawnColumnWorker_Icon)ColumnDef.Worker, pawn);
		}

		public Color GetIconColor(Pawn pawn)
		{
			return getIconColor((PawnColumnWorker_Icon)ColumnDef.Worker, pawn);
		}

		public string GetIconTip(Pawn pawn)
		{
			return getIconTip((PawnColumnWorker_Icon)ColumnDef.Worker, pawn);
		}

		public void ClickedIcon(Pawn pawn)
		{
			clickedIcon((PawnColumnWorker_Icon)ColumnDef.Worker, pawn);
		}

		public void PaintedIcon(Pawn pawn)
		{
			paintedIcon((PawnColumnWorker_Icon)ColumnDef.Worker, pawn);
		}

		public int Padding()
		{
			return padding((PawnColumnWorker_Icon)ColumnDef.Worker);
		}

		public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
		{
			if (!column.IsUniform())
			{
				GuiTools.PushColor(Metrics.GroupHeaderOpacityColor);
				DoMixedValuesIcon(rect);
				GuiTools.PopColor();
			}
            else
            {
				// mostly decompiled code
				var pawn  = GetRepresentingPawn(column.Group.Pawns);

				Texture2D iconFor = GetIconFor(pawn);
				if (iconFor != null)
				{
					Vector2 iconSize = GetIconSize(pawn);
					int num = (int)((rect.width - iconSize.x) / 2f);
					int num2 = Mathf.Max((int)((30f - iconSize.y) / 2f), 0);
					Rect rect2 = new Rect(rect.x + num, rect.y + num2, iconSize.x, iconSize.y);
					var color = GetIconColor(pawn);
					color.a *= Metrics.GroupHeaderOpacityIcon; // adding transparency

					GUI.color = color;
					GUI.DrawTexture(rect2.ContractedBy(Padding()), iconFor);
					GUI.color = Color.white;
					if (Mouse.IsOver(rect2))
					{
						string iconTip = GetIconTip(pawn);
						if (!iconTip.NullOrEmpty())
						{
							TooltipHandler.TipRegion(rect2, iconTip);
						}
					}
					//if (Widgets.ButtonInvisible(rect2, false))
					//{
					//	this.ClickedIcon(pawn);
					//}
					//if (Mouse.IsOver(rect2) && Input.GetMouseButton(0))
					//{
					//	this.PaintedIcon(pawn);
					//}
				}
			}
        }

        public override object GetGroupValue(IEnumerable<Pawn> pawns)
        {
			return null;
        }

        public override void SetGroupValue(IEnumerable<Pawn> pawns, object value)
        {
            
        }

        public override bool CanSetValues()
        {
			return false;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
			return null;
        }

        public override object GetValue(Pawn pawn)
        {
			return GetIconFor(pawn);
        }

		public override void SetValue(Pawn pawn, object value)
		{

		}

		public override bool IsVisible(Pawn pawn)
		{
			return GetIconFor(pawn) != null;
		}

		public override string GetStringValue(Pawn pawn)
		{
			return GetIconFor(pawn)?.name ?? "";
		}

	}
}
