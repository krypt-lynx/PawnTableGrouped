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
using WorkTab;

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
            if (tableName == WorkTabBridge.WorkTabDefName)
            {
                WorkTabBridge.WorkTabRenderEnabled = !active;
            }
        }
    }

    class WorkTabBridge
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

                try
                {
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
                catch
                {
                    disabled = true;
                }
            }
        }

        delegate bool PawnTable_PawnTableOnGUI_Prefix_Delegate(PawnTable arg1, Vector2 arg2, PawnTableDef arg3, ref Vector2 arg4);


        public static void Resolve(bool active)
        {
            if (!active)
            {
                disabled = true;
                return;
            }

            try
            {
                PawnTable_PawnTableOnGUI_prefix = ((PawnTable_PawnTableOnGUI_Prefix_Delegate)PawnTable_PawnTableOnGUI.Prefix).Method;

                PawnTable_RecacheIfDirty_prefix = typeof(PawnTable_RecacheIfDirty).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(bool).MakeByRefType(), typeof(PawnTableDef) }, null);
                PawnTable_RecacheIfDirty_postfix = typeof(PawnTable_RecacheIfDirty).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(bool) }, null);

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
    }
}
