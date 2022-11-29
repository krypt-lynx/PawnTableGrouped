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
using Verse.Sound;

namespace PawnTableGrouped
{
    [StaticConstructorOnStartup] // not it don't. But I need to suppress the issue
    class GroupColumnWorker_CopyPaste: GroupColumnWorker
    {
        static Getter<PawnColumnWorker_CopyPaste, bool> anythingInClipboard = Dynamic.InstanceGetProperty<PawnColumnWorker_CopyPaste, bool>("AnythingInClipboard");
        static Action<PawnColumnWorker_CopyPaste, Pawn> pasteTo = Dynamic.InstanceVoidMethod<PawnColumnWorker_CopyPaste, Pawn>("PasteTo");

        public static readonly Texture2D TexButton_Paste = ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true);

        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            Action pasteAction = null;
            if (this.AnythingInClipboard)
            {
                pasteAction = delegate ()
                {
                    foreach (var pawn in column.Group.Pawns)
                    {
                        PasteTo(pawn);
                    }
                };
            }
            DoPasteButton(new Rect(rect.x, rect.y, 36f, 30f), pasteAction);
        }

        public static void DoPasteButton(Rect rect, Action pasteAction)
        {
            MouseoverSounds.DoRegion(rect);
            Rect rect2 = new Rect(rect.x, rect.y + (rect.height / 2f - 12f), 18f, 24f);

            if (pasteAction != null)
            {
                Rect rect3 = rect2;
                rect3.x = rect2.xMax;
                if (Widgets.ButtonImage(rect3, TexButton_Paste, true))
                {
                    pasteAction();
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegionByKey(rect3, "Paste");
            }
        }

        protected void PasteTo(Pawn pawn) => pasteTo((PawnColumnWorker_CopyPaste)ColumnDef.Worker, pawn);
        protected bool AnythingInClipboard => anythingInClipboard((PawnColumnWorker_CopyPaste)ColumnDef.Worker);

        public override bool CanSetValues() => false;

        public override object DefaultValue(IEnumerable<Pawn> pawns) => null;

        public override object GetValue(Pawn pawn) => null;

        public override void SetValue(Pawn pawn, object value, PawnTable table) { }
    }
}
