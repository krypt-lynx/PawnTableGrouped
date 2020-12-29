using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WildlifeTabAlt
{
    public static class KWidgets
    {
        public static bool DraggableSource(Rect rect, bool draggableValue, out bool checkOn)
        {
            MouseoverSounds.DoRegion(rect);
            bool playSound = false;
            bool wasSet = false;
            checkOn = draggableValue;

            Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(rect, false);
            if (draggableResult == Widgets.DraggableResult.Pressed)
            {
                checkOn = draggableValue;
                wasSet = true;
                playSound = true;
            }
            else if (draggableResult == Widgets.DraggableResult.Dragged)
            {
                // checkOn = !checkOn; // todo:
                //playSound = true;
                //Widgets.checkboxPainting = true;
                // Widgets.checkboxPaintingState = checkOn;
            }
            /*if (Mouse.IsOver(rect) && Widgets.checkboxPainting && Input.GetMouseButton(0) && checkOn != Widgets.checkboxPaintingState)
            {
                checkOn = Widgets.checkboxPaintingState;
                playSound = true;
            }*/ // todo:

            if (playSound)
            {
                if (checkOn)
                {
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                else
                {
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
            }

            return wasSet;
        }

    }
}
