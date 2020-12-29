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
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "PawnTableOnGUI_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "RecacheIfDirty_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "CalculateTotalRequiredHeight"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "CalculateTotalRequiredHeight_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "GetOptimalWidth"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "GetOptimalWidth_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "GetMinWidth"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "GetMinWidth_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "GetMaxWidth"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "GetMaxWidth_prefix"));

        }

        static bool GetOptimalWidth_prefix(PawnTable __instance, PawnColumnDef column, ref float __result)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                __result = groupedTable.override_GetOptimalWidth(column);
                return false;
            }

            return true;
        }

        static bool GetMinWidth_prefix(PawnTable __instance, PawnColumnDef column, ref float __result)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                __result = groupedTable.override_GetMinWidth(column);
                return false;
            }

            return true;
        }

        static bool GetMaxWidth_prefix(PawnTable __instance, PawnColumnDef column, ref float __result)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                __result = groupedTable.override_GetMaxWidth(column);
                return false;
            }

            return true;
        }

        static bool PawnTableOnGUI_prefix(PawnTable __instance, Vector2 position)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                groupedTable.override_PawnTableOnGUI(position);
                return false;
            }

            return true;
        }

        static bool RecacheIfDirty_prefix(PawnTable __instance)
        {
            if (__instance is IPawnTableGrouped groupedTable)
            {
                groupedTable.override_RecacheIfDirty();
                return false;
            }

            return true;
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
