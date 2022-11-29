using HarmonyLib;
using RimWorld;
using RWLayout.alpha2.FastAccess;
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
    public class WorkTabSettingsWorker : ISettingsWorker
    {
        public void TableActiveChanged(string tableName, bool active)
        {
            if (tableName == WorkTabBridge.WorkTabDefName)
            {
                WorkTabBridge.Instance.WorkTabRenderEnabled = !active;
            }
        }
    }

    public class WorkTabBridge : ModBridge<WorkTabBridge>, IWorkTabBridge
    {
        Harmony harmony = null;
        MethodInfo PawnTable_PawnTableOnGUI_prefix = null;
        MethodInfo PawnTable_RecacheIfDirty_prefix = null;
        MethodInfo PawnTable_RecacheIfDirty_postfix = null;
        
        const string workTabHarmonyId = "fluffy.worktab";
        internal const string WorkTabDefName = "Work";


        private bool workTabRenderEnabled = true;
        public bool WorkTabRenderEnabled
        {
            get
            {
                return workTabRenderEnabled;
            }
            set
            {
                if (!Instance.IsActive)
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
                            PatchEnableWorkTabRender(harmony);
                        }
                        else
                        {
                            PatchEnablePTGRender(harmony);
                        }
                        WorktabLayoutDisabled = ForcePatchWorkTab || !WorkTabRenderEnabled;
                    }
                }
                catch (Exception e)
                {
                    Instance.Deactivate();
                    LogHelper.LogException("Exception thrown during tempering with Work Tab harmony patches", e);
                }
            }
        }

        private bool worktabLayoutDisabled = false;
        public bool WorktabLayoutDisabled
        {
            get => worktabLayoutDisabled;
            set
            {
                if (!Instance.IsActive)
                {
                    return;
                }
                try
                {
                    if (worktabLayoutDisabled != value)
                    {
                        worktabLayoutDisabled = value;
                        if (value)
                        {
                            PatchEnableLayoutFix(harmony);
                        }
                        else
                        {
                            PatchDisableLayoutFix(harmony);
                        }
                    }
                }
                catch (Exception e)
                {
                    Instance.Deactivate();
                    LogHelper.LogException("Exception thrown during tempering with Work Tab layout update", e);
                }
            }
        }

        private bool forcePatchWorkTab = false;
        public bool ForcePatchWorkTab {
            get => forcePatchWorkTab;
            set {
                forcePatchWorkTab = value;
                WorktabLayoutDisabled = ForcePatchWorkTab || !WorkTabRenderEnabled;
            }
        }

        delegate bool PawnTable_PawnTableOnGUI_Prefix_Delegate(PawnTable arg1, Vector2 arg2, PawnTableDef arg3, ref Vector2 arg4);

        protected override bool ResolveInternal(Harmony harmony)
        {
            PawnTable_PawnTableOnGUI_prefix = ((PawnTable_PawnTableOnGUI_Prefix_Delegate)PawnTable_PawnTableOnGUI.Prefix).Method;

            PawnTable_RecacheIfDirty_prefix = typeof(PawnTable_RecacheIfDirty).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(bool).MakeByRefType(), typeof(PawnTableDef) }, null);
            PawnTable_RecacheIfDirty_postfix = typeof(PawnTable_RecacheIfDirty).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(PawnTable), typeof(bool) }, null);

            if (PawnTable_PawnTableOnGUI_prefix == null || PawnTable_RecacheIfDirty_prefix == null || PawnTable_RecacheIfDirty_postfix == null)
            {
                return false;
            }

            this.harmony = new Harmony(workTabHarmonyId);


            WorkTabRenderEnabled = !Mod.Settings.pawnTablesEnabled.Contains(WorkTabDefName);
            ForcePatchWorkTab = Mod.Settings.fixWorkTab;

            return true;
        }

        public void PatchEnablePTGRender(Harmony harmony)
        {
            harmony.Unpatch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"), HarmonyPatchType.Prefix, workTabHarmonyId);
            harmony.Unpatch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"), HarmonyPatchType.Prefix, workTabHarmonyId);
            harmony.Unpatch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"), HarmonyPatchType.Postfix, workTabHarmonyId);
        }

        private static void PatchEnableLayoutFix(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(MainTabWindow_WorkTab), nameof(MainTabWindow_WorkTab.RebuildTable)),
                postfix: new HarmonyMethod(typeof(MainTabWindow_WorkTabPatches), nameof(MainTabWindow_WorkTabPatches.RebuildTable_postfix)));
            harmony.Patch(AccessTools.PropertySetter(typeof(PriorityManager), nameof(PriorityManager.ShowScheduler)),
                postfix: new HarmonyMethod(typeof(MainTabWindow_WorkTabPatches), nameof(MainTabWindow_WorkTabPatches.ShowScheduler_postfix)));
            harmony.Patch(AccessTools.Method(typeof(MainTabWindow), "SetInitialSizeAndPosition"),
                postfix: new HarmonyMethod(typeof(MainTabWindow_WorkTabPatches), nameof(MainTabWindow_WorkTabPatches.SetInitialSizeAndPosition_postfix)));
        }

        public void PatchEnableWorkTabRender(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"),
                prefix: new HarmonyMethod(PawnTable_PawnTableOnGUI_prefix));
            harmony.Patch(AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"),
                prefix: new HarmonyMethod(PawnTable_RecacheIfDirty_prefix),
                postfix: new HarmonyMethod(PawnTable_RecacheIfDirty_postfix));
        }

        private static void PatchDisableLayoutFix(Harmony harmony)
        {
            harmony.Unpatch(AccessTools.Method(typeof(MainTabWindow_WorkTab), nameof(MainTabWindow_WorkTab.RebuildTable)),
                HarmonyPatchType.Postfix, workTabHarmonyId);
            harmony.Unpatch(AccessTools.PropertySetter(typeof(PriorityManager), nameof(PriorityManager.ShowScheduler)),
                HarmonyPatchType.Postfix, workTabHarmonyId);
            harmony.Unpatch(AccessTools.Method(typeof(MainTabWindow), "SetInitialSizeAndPosition"),
                HarmonyPatchType.Postfix, workTabHarmonyId);
        }

        public override string ModName()
        {
            return "Work Tab";
        }
    }

    static class MainTabWindow_WorkTabPatches
    {        
        static Action<MainTabWindow_WorkTab> _setDirty = Dynamic.InstanceVoidMethod<MainTabWindow_WorkTab>("SetDirty");

        public static void RebuildTable_postfix(MainTabWindow_WorkTab __instance)
        {
            _setDirty(__instance);
        }

        public static void ShowScheduler_postfix()
        {
            _setDirty(MainTabWindow_WorkTab.Instance);
        }

        internal static void SetInitialSizeAndPosition_postfix(MainTabWindow __instance)
        {
            if (__instance is MainTabWindow_WorkTab workTab) {
                if (PriorityManager.ShowScheduler)
                {
                    try
                    {
                        if (workTab.Table != null)
                        {
                            workTab.RecacheTimeBarRect();
                        }
                    }
                    catch (Exception e)
                    {
                        // I hope the issue is fixed
                        LogHelper.LogException("Work Tab patch failed. Please, report share this log with the author of Grouped Pawns Lists, because the author expects this issue to be fixed\n" +
                            "WorkTab integration is disabled to avoid further issues\n", e);

                        Mod.Settings.fixWorkTab = false;
                        Mod.Settings.pawnTablesEnabled.Remove(WorkTabBridge.WorkTabDefName);

                        WorkTabBridge.Instance.WorkTabRenderEnabled = true;
                        WorkTabBridge.Instance.ForcePatchWorkTab = false;

                        EventBus<PawnTableSettingsChanged>.SendMessage(null, new PawnTableSettingsChanged());

                        WorkTabBridge.Instance.Deactivate();
                    }
                }
            }
        }
    }
}
