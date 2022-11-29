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
        Harmony WorkTabHarmony = null;

        public Type WorkTypeWorkerType { get; private set; } = null;
        public Type WorkGiverWorkerType { get; private set; } = null;

        Getter<MainTabWindow_WorkTab, bool> _anyExpanded = null;

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
                            RenderPatches.Enabled = false;
                        }
                        else
                        {
                            RenderPatches.Enabled = true;
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
                            PatchEnableLayoutFix(WorkTabHarmony);
                        }
                        else
                        {
                            PatchDisableLayoutFix(WorkTabHarmony);
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

        public bool IsWorkTabWindow(MainTabWindow_PawnTable window)
        {
            return window is MainTabWindow_WorkTab;
        }

        public bool Expanded(MainTabWindow_PawnTable window)
        {
            if (window is MainTabWindow_WorkTab workTabWindow) {
                return _anyExpanded(workTabWindow);
            }
            return false;
        }

        delegate bool PawnTable_PawnTableOnGUI_Prefix_Delegate(PawnTable arg1, Vector2 arg2, PawnTableDef arg3, ref Vector2 arg4);

        PatchesBatch RenderPatches;
        PatchesBatch LayoutPatches;

        protected override bool ResolveInternal(Harmony harmony)
        {
            this.WorkTabHarmony = new Harmony(workTabHarmonyId);

            try
            {
                ResolvePatches(WorkTabHarmony);
            }
            catch
            {
                return false;
            }

            _anyExpanded = Dynamic.InstanceGetProperty<MainTabWindow_WorkTab, bool>("AnyExpanded");

            WorkTypeWorkerType = typeof(PawnColumnWorker_WorkType);
            WorkGiverWorkerType = typeof(PawnColumnWorker_WorkGiver);

            WorkTabRenderEnabled = !Mod.Settings.pawnTablesEnabled.Contains(WorkTabDefName);
            ForcePatchWorkTab = Mod.Settings.fixWorkTab;

            return true;
        }

        private void ResolvePatches(Harmony harmony)
        {
            RenderPatches = new PatchesBatch(new PatchInfo[]
            {
                new PatchInfo(true, AccessTools.Method(typeof(PawnTable), "PawnTableOnGUI"),
                    prefix: new HarmonyMethod(((PawnTable_PawnTableOnGUI_Prefix_Delegate)PawnTable_PawnTableOnGUI.Prefix).Method)),

                new PatchInfo(true, AccessTools.Method(typeof(PawnTable), "RecacheIfDirty"),
                    prefix: new HarmonyMethod(typeof(PawnTable_RecacheIfDirty).GetMethod("Prefix",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                        new Type[] { typeof(PawnTable), typeof(bool).MakeByRefType(), typeof(PawnTableDef) }, null)),
                    postfix: new HarmonyMethod(typeof(PawnTable_RecacheIfDirty).GetMethod("Postfix",
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                        new Type[] { typeof(PawnTable), typeof(bool) }, null))),

                new PatchInfo(false, AccessTools.Method(typeof(LabelUtilities), nameof(LabelUtilities.VerticalLabel)),
                    prefix: new HarmonyMethod(typeof(LabelUtilitiesPatches), nameof(LabelUtilitiesPatches.VerticalLabel_prefix))),

                new PatchInfo(false, AccessTools.Method(typeof(DrawUtilities), nameof(DrawUtilities.DrawWorkBoxBackground)),
                    prefix: new HarmonyMethod(typeof(DrawUtilitiesPatches), nameof(DrawUtilitiesPatches.DrawWorkBoxBackground_prefix))),
            }, harmony);

            LayoutPatches = new PatchesBatch(new PatchInfo[]
            {
                new PatchInfo(false, AccessTools.Method(typeof(MainTabWindow_WorkTab), nameof(MainTabWindow_WorkTab.RebuildTable)),
                    postfix: new HarmonyMethod(typeof(MainTabWindow_WorkTabPatches),
                        nameof(MainTabWindow_WorkTabPatches.RebuildTable_postfix))),

                new PatchInfo(false, AccessTools.PropertySetter(typeof(PriorityManager), nameof(PriorityManager.ShowScheduler)),
                    postfix: new HarmonyMethod(typeof(MainTabWindow_WorkTabPatches),
                        nameof(MainTabWindow_WorkTabPatches.ShowScheduler_postfix))),

                new PatchInfo(false, AccessTools.Method(typeof(MainTabWindow), "SetInitialSizeAndPosition"),
                    postfix: new HarmonyMethod(typeof(MainTabWindow_WorkTabPatches),
                        nameof(MainTabWindow_WorkTabPatches.SetInitialSizeAndPosition_postfix))),
            }, harmony);
        }

        public void PatchEnablePTGRender(Harmony harmony)
        {
            $"WorkTabBridge PatchEnablePTGRender".Log();
            RenderPatches.Enabled = true;
        }

        private void PatchEnableLayoutFix(Harmony harmony)
        {
            $"WorkTabBridge PatchEnableLayoutFix".Log();
            LayoutPatches.Enabled = true;
        }

        public void PatchEnableWorkTabRender(Harmony harmony)
        {
            $"WorkTabBridge PatchEnableWorkTabRender".Log();
            RenderPatches.Enabled = false;
        }

        private void PatchDisableLayoutFix(Harmony harmony)
        {
            $"WorkTabBridge PatchDisableLayoutFix".Log();
            LayoutPatches.Enabled = false;
        }

        public override string ModName()
        {
            return "Work Tab";
        }
    }
}
