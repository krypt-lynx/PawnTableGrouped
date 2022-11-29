using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

#if rw_1_4_or_later
namespace PawnTableGrouped
{
    class GroupColumnWorker_ControlGroup : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            rect.yMin += 5;
            Widgets.Dropdown(rect, column, (c) =>
            {
                return (MechanitorControlGroup)(c.IsUniformCached() ? c.GetGroupValueCached() : c.GetDefaultValue());
            },
            Button_GenerateGroupMenu,
            column.IsUniformCached() ? ((MechanitorControlGroup)column.GetGroupValueCached()).Index.ToString() : "mixed",
            paintable: true);

          
            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }


        private IEnumerable<Widgets.DropdownMenuElement<MechanitorControlGroup>> Button_GenerateGroupMenu(PawnTableGroupColumn column)
        {
            var overseers = column.Group.Pawns.Select(x => x.GetOverseer()).Distinct().ToArray();

            if (overseers.Length > 1)
            {
                yield return new Widgets.DropdownMenuElement<MechanitorControlGroup>
                {
                    option = new FloatMenuOption("MultipleOverseersUnableToSet", null),
                    payload = null,
                };
                yield break;
            }

            tmpControlGroups.Clear();
            tmpControlGroups.AddRange(overseers.First().mechanitor.controlGroups);
            var currentControlGroup = column.IsUniformCached() ? (MechanitorControlGroup)column.GetGroupValueCached() : null;

            for (int i = 0; i < tmpControlGroups.Count; i++)
            {
                var localControlGroup = tmpControlGroups[i];
                if (currentControlGroup == localControlGroup)
                {
                    yield return new Widgets.DropdownMenuElement<MechanitorControlGroup>
                    {
                        option = new FloatMenuOption("CannotAssignMechToControlGroup".Translate(localControlGroup.LabelIndexWithWorkMode) + ": " + "AssignMechAlreadyAssigned".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0),
                        payload = localControlGroup
                    };
                }
                else
                {
                    yield return new Widgets.DropdownMenuElement<MechanitorControlGroup>
                    {
                        option = new FloatMenuOption("AssignMechToControlGroup".Translate(localControlGroup.LabelIndexWithWorkMode), delegate ()
                        {
                            column.SetGroupValue(localControlGroup);
                            column.SetDirty();
                        }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0),
                        payload = localControlGroup
                    };
                }
            }
        }

        class _Target {
            public string test = "2";
        }
        class _Payload { }

        private static List<MechanitorControlGroup> tmpControlGroups = new List<MechanitorControlGroup>();

        private IEnumerable<Widgets.DropdownMenuElement<MechanitorControlGroup>> Button_GenerateMenu(Pawn pawn)
        {
            Pawn overseer = pawn.GetOverseer();
            tmpControlGroups.Clear();
            tmpControlGroups.AddRange(overseer.mechanitor.controlGroups);
            MechanitorControlGroup currentControlGroup = pawn.GetMechControlGroup();

            for (int i = 0; i < tmpControlGroups.Count; i++)
            {
                MechanitorControlGroup localControlGroup = tmpControlGroups[i];
                if (currentControlGroup == localControlGroup)
                {
                    yield return new Widgets.DropdownMenuElement<MechanitorControlGroup>
                    {
                        option = new FloatMenuOption("CannotAssignMechToControlGroup".Translate(localControlGroup.LabelIndexWithWorkMode) + ": " + "AssignMechAlreadyAssigned".Translate(), null),
                        payload = localControlGroup
                    };
                }
                else
                {
                    yield return new Widgets.DropdownMenuElement<MechanitorControlGroup>
                    {
                        option = new FloatMenuOption(
                            "AssignMechToControlGroup".Translate(localControlGroup.LabelIndexWithWorkMode),
                            () => {
                                localControlGroup.Assign(pawn);
                            }),
                        payload = localControlGroup
                    };
                }
            }
            tmpControlGroups.Clear();
        }

        public override bool IsVisible(Pawn pawn)
        {
            return !pawn.IsGestating() && pawn.GetOverseer() != null;
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return null;
        }

        public override object GetValue(Pawn pawn)
        {
            Pawn_MechanitorTracker mechanitor = pawn.GetOverseer()?.mechanitor;
            return mechanitor?.GetControlGroup(pawn);
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            var group = (MechanitorControlGroup)value;
            group.Assign(pawn);
        }
    }
}
#endif