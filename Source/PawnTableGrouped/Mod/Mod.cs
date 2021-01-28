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
        
        public static Action ActiveTablesChanged = null;

        public static List<(string packageId, ModBridge bridge)> ModBridges = new List<(string packageId, ModBridge bridge)> {
            ("mehni.numbers", NumbersBridge.Instance),
            ("fluffy.worktab", WorkTabBridge.Instance),
            ("syl.simpleslavery", SimpleSlaveryBridge.Instance),
        };

        public static void RegisterModBridge(string packageId, ModBridge bridge)
        {
            ModBridges.Add((packageId, bridge));
        }

        public static List<GroupWorker> MiscGroupWorkers = new List<GroupWorker>
        {
            new GroupWorker_AllInOne(),
            new GroupWorker_ByRace(),
            new GroupWorker_ByGender(),
            new GroupWorker_ByFaction(),
        };

        public static void RegisterGroupWorker(GroupWorker groupWorker)
        {
            MiscGroupWorkers.Add(groupWorker);
        }

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

        private static IEnumerable<Assembly> AllActiveAssemblies
        {
            get
            {
                yield return Assembly.GetExecutingAssembly();
                foreach (ModContentPack mod in LoadedModManager.RunningMods)
                {
                    for (int i = 0; i < mod.assemblies.loadedAssemblies.Count; i++)
                    {
                        yield return mod.assemblies.loadedAssemblies[i];
                    }
                }
            }
        }

        public static void DetectMods()
        {
            foreach (var modmod in typeof(ModMod).AllSubclasses())
            {
                Activator.CreateInstance(modmod);
            }

            var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageIdPlayerFacing.ToLowerInvariant()).ToHashSet();            

            foreach (var info in ModBridges)
            {
                bool isListed = loadedModIds.Contains(info.packageId);
                $"Enumerating mod bridges: {info.bridge.ModName()}; is listed: {isListed}".Log(MessageType.Message);
                info.bridge.Resolve(isListed, harmony);
            }

            if (SimpleSlaveryBridge.Instance.IsActive)
            {
                RegisterGroupWorker(new GroupWorker_IsSlave());
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
