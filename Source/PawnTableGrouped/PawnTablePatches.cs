using HarmonyLib;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public static class PawnTableExtentions
    {
        static ConditionalWeakTable<PawnTable, PawnTableGroupedImpl> implementations;

        static ConditionalWeakTable<PawnTable, PawnTableGroupedImpl>.CreateValueCallback instantiateTableImpl = table =>
        {
            var def = GetPawnTableDef(table);
            if (Mod.Settings.pawnTablesEnabled.Contains(def.defName))
            {
                return new PawnTableGroupedImpl(table, def);
            }
            else
            {
                return null;
            }
        };

        static PawnTableExtentions()
        {
            ResetImplementationsCache();
            Mod.ActiveTablesChanged = () =>
            {
                ResetImplementationsCache();
            };
        }

        public static void ResetImplementationsCache()
        {
            implementations = new ConditionalWeakTable<PawnTable, PawnTableGroupedImpl>();
        }

        static PawnTableDef GetPawnTableDef(PawnTable table)
        {
            return (PawnTableDef)typeof(PawnTable).GetField("def", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(table);
        }

        public static bool TryGetImplementation(PawnTable table, out PawnTableGroupedImpl implementation)
        {
            implementation = implementations.GetValue(table, instantiateTableImpl);
            return implementation != null;
        }
    }

    public class PawnTablePatches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "PawnTableOnGUI_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "RecacheIfDirty_prefix"));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "CalculateTotalRequiredHeight"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "CalculateTotalRequiredHeight_prefix"));

            harmony.Patch(AccessTools.Method(typeof(Window), "PostClose"),
                prefix: new HarmonyMethod(typeof(PawnTablePatches), "PostClose_postfix"));
        }


        public static PawnTable GetTable(MainTabWindow_PawnTable window)
        {
            return (PawnTable)typeof(MainTabWindow_PawnTable).GetField("table", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(window);
        }

        static void PostClose_postfix(Window __instance)
        {
            if (__instance is MainTabWindow_PawnTable tableWindow)
            {
                var table = GetTable(tableWindow);

                if (table != null && PawnTableExtentions.TryGetImplementation(table, out var impl))
                {
                    impl.SaveData();
                }
            }
        }

        static bool PawnTableOnGUI_prefix(PawnTable __instance, Vector2 position)
        {
            if (PawnTableExtentions.TryGetImplementation(__instance, out var groupedTable))
            {
                groupedTable.PawnTableOnGUI(position);
                return false;
            }

            return true;
        }

        static bool RecacheIfDirty_prefix(PawnTable __instance)
        {
            if (PawnTableExtentions.TryGetImplementation(__instance, out var groupedTable))
            {
                groupedTable.RecacheIfDirty();
                return false;
            }

            return true;
        }


        static bool CalculateTotalRequiredHeight_prefix(PawnTable __instance, ref float __result)
        {
            if (PawnTableExtentions.TryGetImplementation(__instance, out var groupedTable))
            {
                __result = groupedTable.CalculateTotalRequiredHeight();
                return false;
            }

            return true;
        }
    }
}
