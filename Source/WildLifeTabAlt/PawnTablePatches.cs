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
        }

        static ConditionalWeakTable<PawnTable, PawnTableGroupedImpl> implementations = new ConditionalWeakTable<PawnTable, PawnTableGroupedImpl>();

        static bool TryGetImplementation(PawnTable table, out PawnTableGroupedImpl implementation)
        {
            if (table is PawnTable_Wildlife)
            {
                if (!implementations.TryGetValue(table, out implementation)) 
                {
                    implementation = new PawnTableGroupedImpl(table);
                    implementations.Add(table, implementation);
                }
                return true;
            }
            else
            {
                implementation = null;
                return false;
            }
        }

        static bool PawnTableOnGUI_prefix(PawnTable __instance, Vector2 position)
        {
            if (TryGetImplementation(__instance, out var groupedTable))
            {
                groupedTable.PawnTableOnGUI(position);
                return false;
            }

            return true;
        }

        static bool RecacheIfDirty_prefix(PawnTable __instance)
        {
            if (TryGetImplementation(__instance, out var groupedTable))
            {
                groupedTable.RecacheIfDirty();
                return false;
            }

            return true;
        }


        static bool CalculateTotalRequiredHeight_prefix(PawnTable __instance, ref float __result)
        {
            if (TryGetImplementation(__instance, out var groupedTable))
            {
                __result = groupedTable.CalculateTotalRequiredHeight();
                return false;
            }

            return true;
        }
    }
}
