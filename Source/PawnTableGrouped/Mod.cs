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
    public class Settings : ModSettings
    {
        public bool firstRun = true;
        public bool debug = false;
        public bool hideHeaderIfOnlyOneGroup = false;
        public bool usePrimarySortFunction = true;
        public bool groupByColumnExperimental = false;

        public HashSet<string> pawnTablesEnabled = new HashSet<string>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref firstRun, "firstRun2", true);

            Scribe_Values.Look(ref debug, "debug", false);  
            Scribe_Values.Look(ref hideHeaderIfOnlyOneGroup, "hideHeaderIfOnlyOneGroup", false);
            Scribe_Values.Look(ref usePrimarySortFunction, "usePrimarySortFunction", true);
            Scribe_Values.Look(ref groupByColumnExperimental, "groupByColumnExperimental", false);

            Scribe_Collections.Look(ref pawnTablesEnabled, "pawnTablesEnabled");
            if (pawnTablesEnabled == null)
            {
                pawnTablesEnabled = new HashSet<string>();
            }
            base.ExposeData();
        }
    }

    [StaticConstructorOnStartup]
    public static class ModPostInit
    {
        static ModPostInit()
        {
            // enabling supported tables if first run
            if (Mod.Settings.firstRun)
            {
                Mod.Settings.firstRun = false;

                Mod.Settings.pawnTablesEnabled.AddRange(
                    CompatibilityInfoDef.CurrentTables.Where(kvp => kvp.Value.compatibility == TableCompatibility.Supported).Select(kvp => kvp.Key)
                    );
            }
        }
    }

    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }

        private static bool debug = false;
        public static bool Debug { get
            {
                return Settings.debug;
            }
        }

        public static string CommitInfo = null;
        
        public static Verse.Mod Instance = null;
        
        public static Action ActiveTablesChanged = null;

        public Mod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            Settings = GetSettings<Settings>();
            Instance = this;

            Harmony harmony = new Harmony(PackageIdOfMine);

            ApplyPatches(harmony);

            DetectMods();
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
                    CommitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                CommitInfo = null;
            }

            debug = PackageIdOfMine.EndsWith(".dev");
        }

        private static void DetectMods()
        {
            var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageId).ToHashSet();
            
            NumbersWrapper.Resolve(loadedModIds.Contains("mehni.numbers"));
            WorkTabWrapper.Resolve(loadedModIds.Contains("fluffy.worktab"));
            SimpleSlaveryWrapper.Resolve(loadedModIds.Contains("syl.simpleslavery"));
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
