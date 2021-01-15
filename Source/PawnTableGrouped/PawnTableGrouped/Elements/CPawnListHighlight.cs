using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    class CPawnListHighlight : CGuiRoot
    {
        private Pawn pawn;
        private LookTargets target;

        public CPawnListHighlight(Pawn pawn)
        {
            this.pawn = pawn;
            this.target = new LookTargets(pawn);
        }

        public override void DoContent()
        {
            base.DoContent();

            if (Mouse.IsOver(BoundsRounded))
            {
                GUI.DrawTexture(BoundsRounded, TexUI.HighlightTex);
                target.Highlight(true, pawn.IsColonist, false);
            }
        }
    }
}
