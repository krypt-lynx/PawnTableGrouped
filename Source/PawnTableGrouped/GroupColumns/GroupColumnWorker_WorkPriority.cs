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
	public class GCW_WorkPriority_Config
	{
		public int prioritiesCount = 5;
		public int defaultPriority = 3;
		public string priorityColorMethod = "RimWorld.WidgetsWork:ColorOfPriority";

		[Unsaved(false)]
		MethodInfo getColorMethod = null;
		[Unsaved(false)]
		bool getColorMethodFailsafe = false;
		[Unsaved(false)]
		Color getColorMethodFailsafeColor = new Color(0.74f, 0.74f, 0.74f);

		public Color ColorOfPriority(int priority)
		{
			if (getColorMethodFailsafe)
			{
				return getColorMethodFailsafeColor;
			}

			try
			{
				if (getColorMethod == null)
				{
					getColorMethod = HarmonyLib.AccessTools.Method(priorityColorMethod);
				}

				return (Color)getColorMethod.Invoke(null, new object[] { priority });
			}
			catch
			{
				getColorMethodFailsafe = true;
				return getColorMethodFailsafeColor;
			}
		}
	}

	public class GroupColumnWorker_WorkPriority : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            if (!column.IsUniform())
            {
				GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
				DoMixedValuesWidget(rect, column);
				GuiTools.PopColor();
			}
			else
            {
				GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
				var pawn = GetRepresentingPawn(column.Group.Pawns);
                GuiTools.PushFont(GameFont.Medium);
                float x = rect.x + (rect.width - 25f) / 2f;
                float y = rect.y + 2.5f;

                DrawWorkBoxFor(x, y, column, ColumnDef.workType);

                GuiTools.PopFont();

                if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
                {
                    Event.current.Use();
                }
				GuiTools.PopColor();
			}
		}

		protected virtual int PrioritiesCount()
        {
			return 4;
        }

		// Token: 0x060063C0 RID: 25536 RVA: 0x00228E80 File Offset: 0x00227080
		public void DrawWorkBoxFor(float x, float y, PawnTableGroupColumn column, WorkTypeDef wType)
		{
			int prioritiesCount = GetWorkerConfig<GCW_WorkPriority_Config>().prioritiesCount;
			int priority = (int)column.GetGroupValue();

			Rect rect = new Rect(x, y, 25f, 25f);

			DrawWorkBoxBackground(rect, column, wType);
			//GUI.color = Color.white;
			if (Find.PlaySettings.useWorkPriorities)
			{
				if (priority > 0)
				{
					Text.Anchor = TextAnchor.MiddleCenter;
					Color textColor = GetWorkerConfig<GCW_WorkPriority_Config>().ColorOfPriority(priority);
					textColor.a *= GUI.color.a == 1 ? 1 : Metrics.GroupHeaderOpacityText;
					GuiTools.PushColor(textColor);
					Widgets.Label(rect.ContractedBy(-3f), priority.ToStringCached());
					GuiTools.PopColor();
					Text.Anchor = TextAnchor.UpperLeft;
				}
				if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
				{
					int delta = 0;
					if (Event.current.button == 0)
					{
						delta = -1;
					}
					if (Event.current.button == 1)
					{
						delta = 1;

					}
					if (delta != 0)
					{
						int newPriority = (prioritiesCount + priority + delta) % prioritiesCount;

						column.SetGroupValue(newPriority);
						SoundDefOf.DragSlider.PlayOneShotOnCamera(null);

						PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
						PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ManualWorkPriorities, KnowledgeAmount.SmallInteraction);
					}

					Event.current.Use();
					return;
				}
			}
			else
			{
				if (priority > 0)
				{
					GUI.DrawTexture(rect, WidgetsWork.WorkBoxCheckTex);
				}
				if (Widgets.ButtonInvisible(rect, true))
				{
					if (priority > 0)
					{
						column.SetGroupValue(0);
						SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
					}
					else
					{
						column.SetGroupValue(3);
						SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
					}
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
				}
			}
		}

        private void DrawWorkBoxBackground(Rect rect, PawnTableGroupColumn column, WorkTypeDef workDef)
		{
			//float num = p.skills.AverageOfRelevantSkillsFor(workDef);
			Texture2D image;
			//Texture2D image2;
			//float a;
			//if (num < 4f)
			//{
			//	image = WidgetsWork.WorkBoxBGTex_Awful;
			//	image2 = WidgetsWork.WorkBoxBGTex_Bad;
			//	a = num / 4f;
			//}
			//else if (num <= 14f)
			//{
				image = WidgetsWork.WorkBoxBGTex_Bad;
			//	image2 = WidgetsWork.WorkBoxBGTex_Mid;
			//	a = (num - 4f) / 10f;
			//}
			//else
			//{
			//	image = WidgetsWork.WorkBoxBGTex_Mid;
			//	image2 = WidgetsWork.WorkBoxBGTex_Excellent;
			//	a = (num - 14f) / 6f;
			//}
			GUI.DrawTexture(rect, image);
			//GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, a);
			//GUI.DrawTexture(rect, image2);
			//int priority = (int)column.GetGroupValue();

			//if (workDef.relevantSkills.Any<SkillDef>() && num <= 2f && p.workSettings.WorkIsActive(workDef))
			//{
			//	GUI.color = Color.white;
			//	GUI.DrawTexture(rect.ContractedBy(-2f), WidgetsWork.WorkBoxOverlay_Warning);
			//}
			//Passion passion = p.skills.MaxPassionOfRelevantSkillsFor(workDef);
			//if (passion > Passion.None)
			//{
			//	GUI.color = new Color(1f, 1f, 1f, 0.4f);
			//	Rect position = rect;
			//	position.xMin = rect.center.x;
			//	position.yMin = rect.center.y;
			//	if (passion == Passion.Minor)
			//	{
			//		GUI.DrawTexture(position, WidgetsWork.PassionWorkboxMinorIcon);
			//	}
			//	else if (passion == Passion.Major)
			//	{
			//		GUI.DrawTexture(position, WidgetsWork.PassionWorkboxMajorIcon);
			//	}
			//}
			//GUI.color = Color.white;
		}
		
		public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return GetWorkerConfig<GCW_WorkPriority_Config>().defaultPriority;
        }

        public override object GetValue(Pawn pawn)
        {
            return pawn.workSettings.GetPriority(ColumnDef.workType);
        }

        public override bool IsVisible(Pawn pawn)
        {
            return !pawn.WorkTypeIsDisabled(ColumnDef.workType);
        }

        public override void SetValue(Pawn pawn, object value)
        {
            var priority = (int)value;

            if (!pawn.WorkTypeIsDisabled(ColumnDef.workType))
            {
                pawn.workSettings.SetPriority(ColumnDef.workType, priority);
            }
        }
    }
}
