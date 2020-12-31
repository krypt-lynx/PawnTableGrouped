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
            CheckboxDraw(BoundsRounded.xMin, BoundsRounded.center.y - 12, Checked, false);
            GuiTools.PopColor();
            ApplyAll();
            Widgets.Label(BoundsRounded.ContractedBy(new EdgeInsets(0, 0, 0, 5 + 24)), Title);

			bool paintable = true;

			MouseoverSounds.DoRegion(BoundsRounded);
			bool flag = false;
			Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(BoundsRounded, false);
			if (draggableResult == Widgets.DraggableResult.Pressed)
			{
				Checked = !Checked;
				flag = true;
			}
			else if (draggableResult == Widgets.DraggableResult.Dragged && paintable)
			{
				Checked = !Checked;
				flag = true;
                TablesListCheckbox.checkboxPainting = true;
                TablesListCheckbox.checkboxPaintingState = Checked;
			}
			if (paintable && Mouse.IsOver(BoundsRounded) && TablesListCheckbox.checkboxPainting && Input.GetMouseButton(0) && Checked != TablesListCheckbox.checkboxPaintingState)
			{
				Checked = TablesListCheckbox.checkboxPaintingState;
				flag = true;
			}
			if (flag)
			{
				if (Checked)
				{
					SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
				}
				else
				{
					SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
				}
			}



			RestoreAll();

            if (oldChecked != Checked)
            {
                Changed?.Invoke(this, Checked);
            }
        }

        private static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            typeof(Widgets).GetMethod("CheckboxDraw", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { x, y, active, disabled, size, texChecked, texUnchecked });
        }


        private static bool checkboxPainting
        {
            get
            {
                return (bool)typeof(Widgets).GetField("checkboxPainting", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            }
            set
            {
                typeof(Widgets).GetField("checkboxPainting", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
            }
        }

        private static bool checkboxPaintingState
        {
            get
            {
                return (bool)typeof(Widgets).GetField("checkboxPaintingState", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            }
            set
            {
                typeof(Widgets).GetField("checkboxPaintingState", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
            }
        }
    }
}
