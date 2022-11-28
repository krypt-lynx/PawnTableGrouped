using RimWorld;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
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
        static Func<PawnColumnWorker_Text, Pawn, string> _getTextFor = Dynamic.InstanceRetMethod<PawnColumnWorker_Text, Pawn, string>(
            typeof(PawnColumnWorker_Text).GetMethod("GetTextFor", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));
        static Func<PawnColumnWorker_Text, Pawn, string> _getTip = Dynamic.InstanceRetMethod<PawnColumnWorker_Text, Pawn, string>(
            typeof(PawnColumnWorker_Text).GetMethod("GetTip", BindingFlags.NonPublic | BindingFlags.Instance));
#if rw_1_4_or_later
        static Getter<PawnColumnWorker_Text, TextAnchor> _get_Anchor = Dynamic.InstanceGetProperty<PawnColumnWorker_Text, TextAnchor>("Anchor"); 
#endif

        public string GetTextFor(Pawn pawn) => _getTextFor((PawnColumnWorker_Text)ColumnDef.Worker, pawn);
        public string GetTip(Pawn pawn) => _getTip((PawnColumnWorker_Text)ColumnDef.Worker, pawn);

#if rw_1_4_or_later
        public TextAnchor Anchor => _get_Anchor((PawnColumnWorker_Text)ColumnDef.Worker);
#endif

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
                var pawn = GetRepresentingPawn(column.Group.Pawns);
                Rect rect2 = new Rect(rect.x, rect.y, rect.width, Mathf.Min(rect.height, 30f));
                string textFor = GetTextFor(pawn);
                if (textFor != null)
                {
                    var config = GetWorkerConfig<GCW_Text_Config>();

                    Text.Font = GameFont.Small;

                    if (!config.forceAlignment)
                    {
#if rw_1_4_or_later
                        Text.Anchor = Anchor;
#else
                        Text.Anchor = NumbersBridge.IsNumbersTable(table) ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
#endif
                    }
                    else
                    {
                        Text.Anchor = config.textAlignment;
                    }

                    Text.WordWrap = config.wordWrap;
                    GuiTools.PushColor(Metrics.GroupHeaderOpacityColor);
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

        public override void SetGroupValue(IEnumerable<Pawn> pawns, object value, PawnTable table)
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

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
           
        }

        public override bool IsVisible(Pawn pawn)
        {
            return true;
        }
    }
}
