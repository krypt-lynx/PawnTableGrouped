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
    public class GCW_Text_Config
    {
        public TextAnchor textAlignment = TextAnchor.MiddleLeft;
        public bool wordWrap = true;
        public bool forceAlignment = false;
    }

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

        static Color textColor = new Color(1, 1, 1, 0.6f);

        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            if (!column.IsUniform())
            {
                DoMixedValuesIcon(rect);
            }
            else
            {
                var pawn = GetRepresentingPawn(column.Group.Pawns);
                Rect rect2 = new Rect(rect.x, rect.y, rect.width, Mathf.Min(rect.height, 30f));
                string textFor = GetTextFor(pawn);
                if (textFor != null)
                {
                    var config = GetWorkerConfig<GCW_Text_Config>();

                    Text.Font = GameFont.Small;

                    if (!config.forceAlignment && NumbersWrapper.IsActive && NumbersWrapper.NumbersTableType.IsAssignableFrom(table.GetType()))
                    {
                        Text.Anchor = TextAnchor.MiddleCenter;
                    }
                    else
                    {
                        Text.Anchor = config.textAlignment;
                    }
                    Text.WordWrap = config.wordWrap;
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
            return GetTextFor(pawn);
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
