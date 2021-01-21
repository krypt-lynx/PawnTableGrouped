// #if rw_1_1
// #define rw_1_1_or_above
// #define rw_1_1_or_below
// #define rw_1_2_or_below
// #endif

// #if rw_1_2
// #define rw_1_1_or_above
// #define rw_1_2_or_above
// #define rw_1_2_or_below
// #endif

using HarmonyLib;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }

        private static bool debug = false;
        public static bool Debug => Settings.debug;

        static string commitInfo = null;
        public static string CommitInfo => debug ? (commitInfo + "-dev") : commitInfo;
        
        public static Verse.Mod Instance = null;
        
        public static Action ActiveTablesChanged = null;

        static (string packageId, ModBridge bridge)[] ModBridges =  {
            ("mehni.numbers", NumbersBridge.Instance),
            ("fluffy.worktab", WorkTabBridge.Instance),
            ("syl.simpleslavery", SimpleSlaveryBridge.Instance),
            ("derekbickley.ltocolonygroupsfinal", ColonyGroupsBridge.Instance),
        };

        public Mod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            Settings = GetSettings<Settings>();
            Instance = this;

            Harmony harmony = new Harmony(PackageIdOfMine);

            ApplyPatches(harmony);

            DetectMods(harmony);
        }

        private static void ApplyPatches(Harmony harmony)
        {
            PawnTablePatches.Patch(harmony);
        }

        private static void ReadModInfo(ModContentPack content)
        {
            PackageIdOfMine = content.PackageId;

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(name + ".git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    commitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                commitInfo = null;
            }

            debug = PackageIdOfMine.EndsWith(".dev");
        }

        private static void DetectMods(Harmony harmony)
        {
            var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageId).ToHashSet();            

            foreach (var info in ModBridges)
            {
                info.bridge.Resolve(loadedModIds.Contains(info.packageId), harmony);
            }
        }

        public override string SettingsCategory()
        {
            return "Grouped Pawns Lists";
        }

        bool invalidateOnce = false;
        public override void ConstructGui()
        {
            Gui.Embed(Gui.AddElement(new SettingsView()));
            invalidateOnce = true;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (invalidateOnce) // todo: fix CListView visible horizontal scroll bug.
            {
                Gui.SetNeedsUpdateLayout();
                Gui.UpdateLayoutIfNeeded();
                Gui.SetNeedsUpdateLayout();
            }
            base.DoSettingsWindowContents(inRect);
        }

        public static void DoActiveTablesChanged()
        {
            ActiveTablesChanged?.Invoke();
        }
    }


}
