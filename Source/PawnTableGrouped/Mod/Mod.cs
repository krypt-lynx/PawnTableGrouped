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
    public class ModMod { }

    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }

        private static bool debug = false;
        public static bool Debug => Settings.debug;

        static string commitInfo = null;
        public static string CommitInfo => debug ? (commitInfo + "-dev") : commitInfo;
        
        public static Verse.Mod Instance = null;
        
        public static List<(string packageId, ModBridge bridge)> ModBridges = new List<(string packageId, ModBridge bridge)> {
            ("mehni.numbers", NumbersBridge.Instance),
            ("fluffy.worktab", WorkTabBridge.Instance),
        };

        public static void RegisterModBridge(string packageId, ModBridge bridge)
        {
            ModBridges.Add((packageId, bridge));
        }

        public static List<(Func<bool>, GroupWorker)> GroupWorkersConfig = new List<(Func<bool>, GroupWorker)>
        {
            (() => true, new GroupWorker_AllInOne()),
            (() => true, new GroupWorker_ByFaction()),
            (() => true, new GroupWorker_ByRace()),
#if rw_1_4_or_later
            (() => ModsConfig.BiotechActive, new GroupWorker_ByXenotype()),
#endif
            (() => true, new GroupWorker_ByGender()),
#if rw_1_3_or_later
            (() => ModsConfig.IdeologyActive, new GroupWorker_ByIdeo()),
            (() => ModsConfig.IdeologyActive, new GroupWorker_IsSlave()),
#endif
#if rw_1_4_or_later
            (() => ModsConfig.BiotechActive, new GroupWorker_ByControlGroup()),
#endif
        };

        public static List<GroupWorker> groupWorkers = null;

        public static List<GroupWorker> GroupWorkers => 
            groupWorkers ??= GroupWorkersConfig.Where(x => x.Item1()).Select(x => x.Item2).ToList();


        public static void RegisterGroupWorker(GroupWorker groupWorker)
        {
            GroupWorkers.Add(groupWorker);
        }


        public static IEnumerable<string> RunningModInvariantIds =>
            LoadedModManager.RunningMods.Select(x => x.PackageIdPlayerFacing.ToLowerInvariant());

        static Harmony harmony = null;

        public Mod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            Settings = GetSettings<Settings>();
            Instance = this;

            harmony = new Harmony(PackageIdOfMine);

            ApplyPatches(harmony);
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
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name + ".git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    commitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                commitInfo = "no version info found";
            }

            debug = PackageIdOfMine.EndsWith(".dev");
        }

        public static void DetectMods()
        {
            foreach (var modmod in typeof(ModMod).AllSubclasses())
            {
                Activator.CreateInstance(modmod);
            }

            var loadedModIds = RunningModInvariantIds.ToHashSet();            

            foreach (var info in ModBridges)
            {
                bool isListed = loadedModIds.Contains(info.packageId);
                $"Enumerating mod bridges: {info.bridge.ModName()}; is listed: {isListed}".Log();
                info.bridge.Resolve(isListed, harmony);
            }
        }

        public override string SettingsCategory()
        {
            return "Grouped Pawns Lists";
        }

        public override string Version()
        {
            return Mod.CommitInfo;
        }

        bool invalidateOnce = false;
        public override CElement CreateSettingsView()
        {
            invalidateOnce = true;
            return new SettingsView();
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
    }


}
