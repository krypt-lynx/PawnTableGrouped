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
    public class GroupColumnWorker_MedicalCare : GroupColumnWorker
    {
        static GroupColumnWorker_MedicalCare()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                careTextures = new Resource<Texture2D>[] {
                    new Resource<Texture2D>("UI/Icons/Medical/NoCare"),
                    new Resource<Texture2D>("UI/Icons/Medical/NoMeds"),
                    ThingDefOf.MedicineHerbal.uiIcon,
                    ThingDefOf.MedicineIndustrial.uiIcon,
                    ThingDefOf.MedicineUltratech.uiIcon,
                };
            });
        }

        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);

            if (!column.IsUniform())
            {
                DoMixedValuesWidget(rect, column); // todo:  Widgets.Dropdown, but need a texture
            }
            else
            {

                Widgets.Dropdown(rect, column, (c) => (MedicalCareCategory)c.GetGroupValue(), (c) => MedicalCareSelectButton_GenerateMenu(c), null, careTextures[(int)(MedicalCareCategory)column.GetGroupValue()], null, null, null, true);
            }

            GuiTools.PopColor();
            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }

            /*
			Widgets.Dropdown<Pawn, MedicalCareCategory>(rect, pawn, new Func<Pawn, MedicalCareCategory>(MedicalCareUtility.MedicalCareSelectButton_GetMedicalCare), new Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>>>(MedicalCareUtility.MedicalCareSelectButton_GenerateMenu), null, MedicalCareUtility.careTextures[(int)pawn.playerSettings.medCare], null, null, null, true);
            */
        }

        private static new Resource<Texture2D>[] careTextures;

        // Token: 0x06004A50 RID: 19024 RVA: 0x00192AC2 File Offset: 0x00190CC2

        private static IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>> MedicalCareSelectButton_GenerateMenu(PawnTableGroupColumn column)
        {
            int num;
            for (int i = 0; i < 5; i = num + 1)
            {
                MedicalCareCategory mc = (MedicalCareCategory)i;
                yield return new Widgets.DropdownMenuElement<MedicalCareCategory>
                {
                    option = new FloatMenuOption(mc.GetLabel(), delegate ()
                    {
                        column.SetGroupValue(mc);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null),
                    payload = mc
                };
                num = i;
            }
            yield break;
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            var pawn = GetRepresentingPawn(pawns);

            if (pawn != null) {
                if (pawn.Faction == Faction.OfPlayer)
                {
                    if (pawn.RaceProps.Animal)
                    {
                        return Find.PlaySettings.defaultCareForColonyAnimal;                        
                    }
                    if (!pawn.IsPrisoner)
                    {
                        return Find.PlaySettings.defaultCareForColonyHumanlike;
                    }
                    return Find.PlaySettings.defaultCareForColonyPrisoner;
                }
                else
                {
                    if (pawn.Faction == null && pawn.RaceProps.Animal)
                    {
                        return Find.PlaySettings.defaultCareForNeutralAnimal;
                    }
                    if (pawn.Faction == null || !pawn.Faction.HostileTo(Faction.OfPlayer))
                    {
                        return Find.PlaySettings.defaultCareForNeutralFaction;
                    }
                    return Find.PlaySettings.defaultCareForHostileFaction;
                }
            } 
            else
            {
                return MedicalCareCategory.HerbalOrWorse;
            }
        }

        public override object GetValue(Pawn pawn)
        {
            return pawn.playerSettings?.medCare;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.medCare = (MedicalCareCategory)value;
            }
        }

        public override bool IsVisible(Pawn pawn)
        {
            return true;
        }

    }
}
