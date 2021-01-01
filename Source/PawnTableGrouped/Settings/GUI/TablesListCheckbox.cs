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
using Verse.Sound;

namespace PawnTableGrouped
{
    class TablesListCheckbox : CTitledElement
    {
        public bool Checked = false;
        public Action<TablesListCheckbox, bool> Changed = null;

        public override Vector2 tryFit(Vector2 size)
        {
            return tryFitText(size, new Vector2(5 + 24, 0));
        }

        public override void DoContent()
        {
            base.DoContent();
            bool oldChecked = Checked;

            GuiTools.PushColor(UnityEngine.Color.white);
            Widgets.Checkbox(BoundsRounded.xMin, BoundsRounded.center.y - 12, ref Checked, paintable: true);
            GuiTools.PopColor();
            ApplyAll();
            Widgets.Label(BoundsRounded.ContractedBy(new EdgeInsets(0, 0, 0, 5 + 24)), Title);
			RestoreAll();

            if (oldChecked != Checked)
            {
                Changed?.Invoke(this, Checked);
            }
        }
    }
}
