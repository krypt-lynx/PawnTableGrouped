﻿using HarmonyLib;
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
        public bool hideHeaderIfOnlyOneGroup = true;
        public bool debug = false;

        public HashSet<string> pawnTablesEnabled = new HashSet<string>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref firstRun, "firstRun", true);

            Scribe_Values.Look(ref hideHeaderIfOnlyOneGroup, "hideHeaderIfOnlyOneGroup", false);
            Scribe_Values.Look(ref debug, "debug", false);

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
            var info = DefDatabase<CompatibilityInfoDef>.GetNamed("ModCompatibility");
            var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageId).ToHashSet();

            Mod.DefaultTableConfig.Clear();

            foreach (var tableInfo in info.compatibilityList.Where(x => loadedModIds.Contains(x.packageId)).SelectMany(x => x.tables))
            {
                Mod.DefaultTableConfig[tableInfo.name] = tableInfo.defaultGrouping;
            }

            if (Mod.Settings.firstRun)
            {
                Mod.Settings.firstRun = false;
                Mod.Settings.pawnTablesEnabled.AddRange(
                    info.compatibilityList
                    .SelectMany(x => x.tables)
                    .Where(x => x.compatibility == TableCompatibility.Supported)
                    .Select(x => x.name));
            }
        }
    }

    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }
        public static Dictionary<string, string> DefaultTableConfig = new Dictionary<string, string>();

        private static bool debug = false;
        public static bool Debug { get
            {
                return debug || Settings.debug;
            }
        }

        public static string CommitInfo = null;
        public static bool ModNumbersActive = false;
        
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
            if (loadedModIds.Contains("mehni.numbers"))
            {
                ModNumbersActive = true;
            }

            NumbersWrapper.Resolve();
        }

        public override string SettingsCategory()
        {
            return "Grouped Pawns Lists";
        }

        public override void ConstructGui()
        {
            Gui.Embed(Gui.AddElement(new SettingsView()));
        }

        public static void DoActiveTablesChanged()
        {
            ActiveTablesChanged?.Invoke();
        }
    }


}
