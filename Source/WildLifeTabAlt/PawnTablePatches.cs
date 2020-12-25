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
            //harmony.Patch(AccessTools.Method(typeof(PawnColumnWorker), "Compare"),
            //    prefix: new HarmonyMethod(typeof(PawnTablePatches), "Compare_prefix"));
        }

        public static void RegisterPawnTableExtension(PawnTable table, PawnTableGrouped extension)
        {
            extensions.Add(table, extension);
        }

        static ConditionalWeakTable<PawnTable, PawnTableGrouped> extensions = new ConditionalWeakTable<PawnTable, PawnTableGrouped>();

        static bool PawnTableOnGUI_prefix(PawnTable __instance, Vector2 position)
        {
            PawnTableGrouped extension;
            if (extensions.TryGetValue(__instance, out extension))
            {
                extension.override_PawnTableOnGUI(position);
                return false;
            }

            return true;
        }

        static bool RecacheIfDirty_prefix(PawnTable __instance)
        {
            PawnTableGrouped extension;
            if (extensions.TryGetValue(__instance, out extension))
            {
                extension.override_RecacheIfDirty();
                return false;
            }

            return true;
        }
    }
}
