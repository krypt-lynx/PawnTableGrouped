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
        public Texture2D GetIconFor(Pawn pawn)
        {
            return (Texture2D)typeof(PawnColumnWorker_Icon).GetMethod("GetIconFor", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ColumnDef.Worker, new object[] { pawn });
        }

		public Vector2 GetIconSize(Pawn pawn)
		{
			return (Vector2)typeof(PawnColumnWorker_Icon).GetMethod("GetIconSize", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ColumnDef.Worker, new object[] { pawn });
		}

		public Color GetIconColor(Pawn pawn)
		{
			return (Color)typeof(PawnColumnWorker_Icon).GetMethod("GetIconColor", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ColumnDef.Worker, new object[] { pawn });
		}

		public string GetIconTip(Pawn pawn)
		{
			return (string)typeof(PawnColumnWorker_Icon).GetMethod("GetIconTip", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ColumnDef.Worker, new object[] { pawn });
		}

		public void ClickedIcon(Pawn pawn)
		{
			typeof(PawnColumnWorker_Icon).GetMethod("ClickedIcon", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ColumnDef.Worker, new object[] { pawn });
		}

		public void PaintedIcon(Pawn pawn)
		{
			typeof(PawnColumnWorker_Icon).GetMethod("PaintedIcon", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ColumnDef.Worker, new object[] { pawn });
		}

		public int Padding()
		{
			return (int)typeof(PawnColumnWorker_Icon).GetProperty("Padding", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ColumnDef.Worker);
		}

		public override void DoCell(Rect rect, PawnTableGroup group, PawnTable table, int columnIndex)
		{
			if (!group.IsUniform(columnIndex))
			{
				DoMixedValuesIcon(rect);
			}
            else
            {
				// mostly decompiled code
				var pawn  = group.Pawns.First();

				Texture2D iconFor = GetIconFor(pawn);
				if (iconFor != null)
				{
					Vector2 iconSize = GetIconSize(pawn);
					int num = (int)((rect.width - iconSize.x) / 2f);
					int num2 = Mathf.Max((int)((30f - iconSize.y) / 2f), 0);
					Rect rect2 = new Rect(rect.x + num, rect.y + num2, iconSize.x, iconSize.y);
					var color = GetIconColor(pawn);
					color.a *= 0.4f; // adding transparency
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
			return true;
		}

	}
}
