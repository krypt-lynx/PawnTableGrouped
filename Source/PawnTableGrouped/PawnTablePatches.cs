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
    public class StringListDef : Def
    {
        public List<string> list;
    }

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

        static PawnTablePatches()
        {
            ResetImplementationsCache();
            Mod.ActiveTablesChanged = () =>
            {
                ResetImplementationsCache();
            };
        }
        
        static ConditionalWeakTable<PawnTable, PawnTableGroupedImpl> implementations;

        static ConditionalWeakTable<PawnTable, PawnTableGroupedImpl>.CreateValueCallback instantiateTableImpl = table =>
        {
            var def = GetPawnTableDef(table).defName;
            if (Mod.Settings.pawnTablesEnabled.Contains(def))
            {
                return new PawnTableGroupedImpl(table);
            }
            else
            {
                return null;
            }
        };

        public static void ResetImplementationsCache()
        {
            implementations = new ConditionalWeakTable<PawnTable, PawnTableGroupedImpl>();
        }

        static PawnTableDef GetPawnTableDef(PawnTable table)
        {
            return (PawnTableDef)typeof(PawnTable).GetField("def", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(table);
        }

        static bool TryGetImplementation(PawnTable table, out PawnTableGroupedImpl implementation)
        {
            implementation = implementations.GetValue(table, instantiateTableImpl);

            return implementation != null;
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
