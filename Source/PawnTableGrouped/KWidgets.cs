using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PawnTableGrouped
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


		public static void DoAllowedAreaSelectors(Rect rect, PawnTableGroupColumn column, Func<Area, string> getAreaTitle)
		{
			if (Find.CurrentMap == null)
			{
				return;
			}
			List<Area> allAreas = Find.CurrentMap.areaManager.AllAreas;
			int areasCount = 1;
			for (int i = 0; i < allAreas.Count; i++)
			{
				if (allAreas[i].AssignableAsAllowed())
				{
					areasCount++;
				}
			}
			float zoneWidth = rect.width / areasCount;
			Text.WordWrap = false;
			Text.Font = GameFont.Tiny;
			DoAreaSelector(new Rect(rect.x + 0f, rect.y, zoneWidth, rect.height), column, null, getAreaTitle);
			int num3 = 1;
			for (int j = 0; j < allAreas.Count; j++)
			{
				if (allAreas[j].AssignableAsAllowed())
				{
					float num4 = (float)num3 * zoneWidth;
					DoAreaSelector(new Rect(rect.x + num4, rect.y, zoneWidth, rect.height), column, allAreas[j], getAreaTitle);
					num3++;
				}
			}
			Text.WordWrap = true;
			Text.Font = GameFont.Small;
		}

		// Token: 0x06005B24 RID: 23332 RVA: 0x001E4F30 File Offset: 0x001E3130
		private static void DoAreaSelector(Rect rect, PawnTableGroupColumn column, Area area, Func<Area, string> getAreaTitle)
		{
			MouseoverSounds.DoRegion(rect);
			rect = rect.ContractedBy(1f);
			GUI.DrawTexture(rect, (area != null) ? area.ColorTexture : BaseContent.GreyTex);
			Text.Anchor = TextAnchor.MiddleLeft;
			string text = getAreaTitle(area);
			Rect rect2 = rect;
			rect2.xMin += 3f;
			rect2.yMin += 2f;
			Widgets.Label(rect2, text);

			var currentArea = ((AreaData)column.GetGroupValue()).Area;
			var isUniform = column.IsUniform();
			if (column.IsUniform())
            {
				if (currentArea == area)
				{
					Widgets.DrawBox(rect, 2);
				}
			}

			if (Event.current.rawType == EventType.MouseUp && Event.current.button == 0)
			{
				dragging = false;
			}
			if (!Input.GetMouseButton(0) && Event.current.type != EventType.MouseDown)
			{
				dragging = false;
			}
			if (Mouse.IsOver(rect))
			{
				if (area != null)
				{
					area.MarkForDraw();
				}
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
				{
					dragging = true;
				}
				if (dragging && (!isUniform || currentArea != area))
				{
					column.SetGroupValue(new AreaData(area));
					SoundDefOf.Designate_DragStandard_Changed.PlayOneShotOnCamera(null);
				}
			}
			Text.Anchor = TextAnchor.UpperLeft;
			TooltipHandler.TipRegion(rect, text);
		}

		static bool dragging = false;
	}
}
