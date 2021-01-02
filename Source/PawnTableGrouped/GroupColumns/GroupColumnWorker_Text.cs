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
    public class GroupColumnWorker_Text : GroupColumnWorker
    {
        public string GetTextFor(Pawn pawn)
        {
            return (string)typeof(PawnColumnWorker_Text).GetMethod("GetTextFor", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((PawnColumnWorker_Text)ColumnDef.Worker, new object[] { pawn });
        }

        public string GetTip(Pawn pawn)
        {
            return (string)typeof(PawnColumnWorker_Text).GetMethod("GetTip", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((PawnColumnWorker_Text)ColumnDef.Worker, new object[] { pawn });
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => GetTextFor(p));
        }

        static Color textColor = new Color(1, 1, 1, 0.6f);

        public override void DoCell(Rect rect, PawnTableGroup group, PawnTable table, int columnIndex)
        {
            base.DoCell(rect, group, table, columnIndex);
            if (!group.InUniform(columnIndex))
            {
                DoMixedValuesIcon(rect);
            }
            else
            {
                var pawn = group.Pawns.First();
                Rect rect2 = new Rect(rect.x, rect.y, rect.width, Mathf.Min(rect.height, 30f));
                string textFor = GetTextFor(pawn);
                if (textFor != null)
                {
                    Text.Font = GameFont.Small;

                    if (NumbersWrapper.NumbersTableType.IsAssignableFrom(table.GetType()))
                    {
                        Text.Anchor = TextAnchor.MiddleCenter;
                    }
                    else
                    {
                        Text.Anchor = TextAnchor.MiddleLeft;
                    }
                    Text.WordWrap = false;
                    GuiTools.PushColor(textColor);
                    Widgets.Label(rect2, textFor);
                    GuiTools.PopColor();
                    Text.WordWrap = true;
                    Text.Anchor = TextAnchor.UpperLeft;
                    if (Mouse.IsOver(rect2))
                    {
                        string tip = GetTip(pawn);
                        if (!tip.NullOrEmpty())
                        {
                            TooltipHandler.TipRegion(rect2, tip);
                        }
                    }
                }
            }
        }

        public override object GetGroupValue(IEnumerable<Pawn> pawns)
        {
            var pawn = pawns.FirstOrDefault();
            return pawn != null ? GetTextFor(pawn) : null;
        }

        public override void SetGroupValue(IEnumerable<Pawn> pawns, object value)
        {

        }

        public override bool CanSetValues()
        {
            return false;
        }

        public override object DefaultValue()
        {
            return null;
        }

        public override object GetValue(Pawn pawn)
        {
            return GetTextFor(pawn);
        }

        public override void SetValue(Pawn pawn, object value)
        {
           
        }

        public override bool IsVisible(IEnumerable<Pawn> pawns)
        {
            return true;
        }
    }
}
