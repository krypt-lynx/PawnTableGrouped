using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public interface ISettingsWorker
    {
        void TableActiveChanged(string tableName, bool active);
    }

    class WorkTabSettingsWorker : ISettingsWorker
    {
        public void TableActiveChanged(string tableName, bool active)
        {
            if (tableName == WorkTabWrapper.WorkTabDefName)
            {
                WorkTabWrapper.WorkTabRenderEnabled = !active;
            }
        }
    }

    class WorkTabWrapper
    {
        static bool disabled = true;
        static Harmony harmony = null;
        static MethodInfo PawnTable_PawnTableOnGUI_prefix = null;
        static MethodInfo PawnTable_RecacheIfDirty_prefix = null;
        static MethodInfo PawnTable_RecacheIfDirty_postfix = null;
        
        const string workTabHarmonyId = "fluffy.worktab";
        internal const string WorkTabDefName = "Work";

        public static bool IsActive
        {
            get
            {
                return !disabled;
            }
        }

        public static bool workTabRenderEnabled = true;
        public static bool WorkTabRenderEnabled
        {
            get
            {
                return workTabRenderEnabled;
            }
            set
            {
                if (disabled)
                {
                    return;
                }

                if (workTabRenderEnabled != value)
                {
                    workTabRenderEnabled = value;
                    if (value)
                    {
                        harmony.Patch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"),
                            prefix: new HarmonyMethod(PawnTable_PawnTableOnGUI_prefix));
                        harmony.Patch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"),
                            prefix: new HarmonyMethod(PawnTable_RecacheIfDirty_prefix),
                            postfix: new HarmonyMethod(PawnTable_RecacheIfDirty_postfix));
                    } 
                    else
                    {
                        harmony.Unpatch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"), HarmonyPatchType.Prefix, workTabHarmonyId);
                        harmony.Unpatch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"), HarmonyPatchType.Prefix, workTabHarmonyId);
                        harmony.Unpatch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"), HarmonyPatchType.Postfix, workTabHarmonyId);
                    }
                }
            }
        }

        public static void Resolve(bool active)
        {
            if (!active)
            {
                disabled = true;
                return;
            }

            try
            {
                Type PawnTable_PawnTableOnGUI = GenTypes.GetTypeInAnyAssembly("WorkTab.PawnTable_PawnTableOnGUI");
                Type PawnTable_RecacheIfDirty = GenTypes.GetTypeInAnyAssembly("WorkTab.PawnTable_RecacheIfDirty");

                if (PawnTable_PawnTableOnGUI == null || PawnTable_RecacheIfDirty == null)
                {
                    throw new Exception();
                }

                PawnTable_PawnTableOnGUI_prefix = PawnTable_PawnTableOnGUI.GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(Vector2), typeof(PawnTableDef), typeof(Vector2).MakeByRefType() }, null);


                PawnTable_RecacheIfDirty_prefix = PawnTable_RecacheIfDirty.GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(bool).MakeByRefType(), typeof(PawnTableDef) }, null);
                PawnTable_RecacheIfDirty_postfix = PawnTable_RecacheIfDirty.GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(bool) }, null);

                if (PawnTable_PawnTableOnGUI_prefix == null || PawnTable_RecacheIfDirty_prefix == null || PawnTable_RecacheIfDirty_postfix == null)
                {
                    throw new Exception();
                }

                harmony = new Harmony(workTabHarmonyId);

                disabled = false;

                WorkTabRenderEnabled = !Mod.Settings.pawnTablesEnabled.Contains(WorkTabDefName);
            }
            catch
            {
                disabled = true;
            }
        }

        /*
        private static void PawnTable_RecacheIfDirty_prefix_Bridge(PawnTable __instance, ref bool __state, PawnTableDef ___def)
        {
            if (IsActive)
            {
                object[] args = { __instance, __state, ___def };
                PawnTable_RecacheIfDirty_prefix.Invoke(null, args);
                __state = (bool)args[1];
            }
        }

        private static void PawnTable_RecacheIfDirty_postfix_Bridge(PawnTable __instance, bool __state)
        {
            if (IsActive)
            {
                PawnTable_RecacheIfDirty_postfix.Invoke(null, new object[] { __instance, __state });
            }
        }

        private static bool PawnTable_PawnTableOnGUI_Prefix_Bridge(PawnTable __instance,
                           Vector2 position,
                           PawnTableDef ___def,
                           ref Vector2 ___scrollPosition)
        {
            if (IsActive)
            {
                object[] args = { __instance, position, ___def, ___scrollPosition };
                var result = (bool)PawnTable_PawnTableOnGUI_prefix.Invoke(null, args);
                ___scrollPosition = (Vector2)args[3];
                return result;
            }
            return true;
        }*/
    }
}
