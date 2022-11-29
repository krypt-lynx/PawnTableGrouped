using RimWorld;
using RWLayout.alpha2.FastAccess;
using System;
using UnityEngine;
using Verse;
using WorkTab;

namespace PawnTableGrouped
{
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

        public static void SetInitialSizeAndPosition_postfix(MainTabWindow __instance)
        {
            if (__instance is MainTabWindow_WorkTab workTab)
            {
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

    static class LabelUtilitiesPatches
    {
        public static bool VerticalLabel_prefix(Rect rect, string text, float margin)
        {
            RenderHelper.VerticalLabel(rect, text);
            return false;
        }
    }

    static class DrawUtilitiesPatches
    {
        internal static bool DrawWorkBoxBackground_prefix(Rect box, Pawn pawn, WorkTypeDef worktype)
        {
            RenderHelper.DrawWorkBoxBackground(box, pawn, worktype);
            return false;
        }
    }

    static class GUIClipPatches
    {

    }
}
