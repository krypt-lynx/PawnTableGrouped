using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PawnTableGrouped
{
    public abstract class GroupColumnWorker_WorkBase : GroupColumnWorker
    {
		static Color outlineColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);
		static float outlineMargin = 2f;

		public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
		{
			var boxRect = BoxRext(rect);

			bool mouseOver = Mouse.IsOver(rect);

			DrawWorkBoxFor(boxRect, rect, mouseOver, column, ColumnDef.workType);
		}

        public void DrawWorkBoxFor(Rect rect, Rect outerRect, bool mouseOver, PawnTableGroupColumn column, WorkTypeDef wType)
		{
			if (!mouseOver)
            {
				rect = rect.ContractedBy(outlineMargin);
            }

			DrawWorkBoxBackground(rect);

			if (!mouseOver && !AlwaysShowPriority())
			{
				return;
			}

			int prioritiesCount = GetPrioritiesCount();
			bool uniform = column.IsUniformCached();
			int priority = uniform ? (int)column.GetGroupValueCached() : (int)column.GetDefaultValue();

			//GUI.color = Color.white;
			if (Find.PlaySettings.useWorkPriorities)
            {
                if (priority > 0)
                {
                    if (uniform)
                    {
                        Text.Anchor = TextAnchor.MiddleCenter;
                        Color textColor = GetWorkerConfig<GCW_WorkPriority_Config>().ColorOfPriority(priority);
                        textColor.a *= mouseOver ? 1 : Metrics.GroupHeaderOpacityText;

                        GuiTools.PushFont(mouseOver ? TextFont() : SmallerTextFont());
                        GuiTools.PushColor(textColor);
                        Widgets.Label(rect.ContractedBy(-3f), priority.ToStringCached());
                        GuiTools.PopColor();
                        GuiTools.PopFont();
                        Text.Anchor = TextAnchor.UpperLeft;
                    }
                    else
                    {
                        DrawMixedValues(rect, mouseOver);
                    }

                }

                HandleEvents(rect, column, mouseOver, uniform, prioritiesCount, priority);
            }
            else
            {
                if (priority > 0)
                {
                    if (uniform)
                    {
                        GuiTools.PushColor(mouseOver ? Color.white : Metrics.GroupHeaderOpacityIconColor);
                        GUI.DrawTexture(rect, WidgetsWork.WorkBoxCheckTex);
                        GuiTools.PopColor();
                    }
                    else
                    {
                        DrawMixedValues(rect, mouseOver);
                    }
                }

                HandleCheckboxEvents(outerRect, column, priority);
            }
        }

        private void HandleCheckboxEvents(Rect rect, PawnTableGroupColumn column, int priority)
        {
            if (Widgets.ButtonInvisible(rect, true))
            {
                if (priority > 0)
                {
                    column.SetGroupValue(0);
                    if (PlaySounds())
                    {
                        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                    }
                }
                else
                {
                    if (PlaySounds())
                    {
                        column.SetGroupValue(3);
                    }
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
            }
        }

        private void DrawMixedValues(Rect rect, bool mouseOver)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GuiTools.PushFont(mouseOver ? SmallerTextFont() : SmallestTextFont());
			var color = Color.grey;
			color.a *= mouseOver ? 1 : Metrics.GroupHeaderOpacityText;
			GuiTools.PushColor(color);
            Widgets.Label(rect.ContractedBy(-3f), "…");
            GuiTools.PopColor();
            GuiTools.PopFont();
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void HandleEvents(Rect rect, PawnTableGroupColumn column, bool mouseOver, bool uniform, int prioritiesCount, int priority)
		{
			bool needToSet = false;
			int delta = 0;
			if (Event.current.type == EventType.MouseDown && mouseOver)
			{
				if (Event.current.button == 0)
				{
					needToSet = true;
					delta = uniform ? -1 : 0;
					Event.current.Use();
				}
				if (Event.current.button == 1)
				{
					needToSet = true;
					delta = uniform ? 1 : 0;
					Event.current.Use();
				}
			}

			if (Event.current.type == EventType.ScrollWheel && mouseOver && UseMouseScroll())
            {
				needToSet = true;
				delta = uniform ? Math.Sign(Event.current.delta.y) : 0;
				Event.current.Use();
			}

			if (needToSet)
			{
				int newPriority = (prioritiesCount + priority + delta) % prioritiesCount;

				column.SetGroupValue(newPriority);
				if (PlaySounds())
				{
					SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
				}
			}

		}

        private void DrawWorkBoxBackground(Rect rect)
		{
			GuiTools.PushColor(outlineColor);
			GuiTools.Box(rect, EdgeInsets.One);
			GuiTools.PopColor();
		}

		public override bool CanSetValues() => true;
        public override bool IsStaticVisible() => true;

        protected abstract Rect BoxRext(Rect cellRect);
		protected abstract GameFont TextFont();
		protected abstract GameFont SmallerTextFont();
		protected abstract GameFont SmallestTextFont();

		protected abstract int GetPrioritiesCount();

		protected abstract bool PlaySounds();
		protected abstract bool UseMouseScroll();
		protected abstract bool AlwaysShowPriority();

	}
}
