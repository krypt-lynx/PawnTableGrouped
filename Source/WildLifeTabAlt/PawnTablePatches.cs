using HarmonyLib;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt
{
    class PawnTablePatches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "PawnTableOnGUI_prefix"),
                postfix: new HarmonyMethod(typeof(PawnTablePatches), "PawnTableOnGUI_postfix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "RecacheIfDirty_prefix"),
                postfix: new HarmonyMethod(typeof(PawnTablePatches), "RecacheIfDirty_postfix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "CalculateTotalRequiredHeight"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "CalculateTotalRequiredHeight_prefix"));
        }

        static bool PawnTableOnGUI_prefix(PawnTable __instance, Vector2 position)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {                
                return groupedTable.override_PawnTableOnGUI_Prefix(position);
            }

            return true;
        }

        static void PawnTableOnGUI_postfix(PawnTable __instance, Vector2 position)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                groupedTable.override_PawnTableOnGUI_Postfix(position);
            }
        }

        static bool RecacheIfDirty_prefix(PawnTable __instance, ref bool __state)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                return groupedTable.override_RecacheIfDirty_Prefix(out __state);
            }

            return true;
        }

        static void RecacheIfDirty_postfix(PawnTable __instance, bool __state)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                groupedTable.override_RecacheIfDirty_Postfix(__state);
            }
        }


        static bool CalculateTotalRequiredHeight_prefix(PawnTable __instance, ref float __result)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                __result = groupedTable.override_CalculateTotalRequiredHeight();
                return false;
            }

            return true;
        }
    }
}
