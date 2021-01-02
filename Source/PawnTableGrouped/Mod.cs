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
        public bool groupedAnimalsTab = true;
        public bool groupedWildlifeTab = true;
        public bool hideHeaderIfOnlyOneGroup = true;
        
        public bool interactiveGroupHeaderExperimental = false;

        public HashSet<string> pawnTablesEnabled = new HashSet<string>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref firstRun, "firstRun", true);

            Scribe_Values.Look(ref groupedAnimalsTab, "groupedAnimalsTab", true);
            Scribe_Values.Look(ref groupedWildlifeTab, "groupedWildlifeTab", true);
            Scribe_Values.Look(ref hideHeaderIfOnlyOneGroup, "hideHeaderIfOnlyOneGroup", false);
            Scribe_Values.Look(ref interactiveGroupHeaderExperimental, "interactiveGroupHeaderExperimental", false);

            Scribe_Collections.Look(ref pawnTablesEnabled, "pawnTablesEnabled");
            if (pawnTablesEnabled == null)
            {
                pawnTablesEnabled = new HashSet<string>();
            }
            base.ExposeData();
        }
    }

    public class NumbersWrapper
    {
        static bool disabled = true;
        static Type numbersType = null;
        static Type pawnTableType = null;

        public static Type NumbersTableType { get
            {
                if (disabled)
                {
                    return null;
                }

                return pawnTableType;
            }
        }

        public static int ReorderableGroup(PawnTable pawnTable)
        {
            if (disabled)
            {
                return 0;
            }

            try
            {
                return (int)numbersType.GetMethod("ReorderableGroup", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { pawnTable });
            } 
            catch
            {
                disabled = true;
                return 0;
            }
        }

        public static void CallReorderableWidget(int groupId, Rect rect)
        {
            if (disabled)
            {
                return;
            }

            try
            {
                numbersType.GetMethod("CallReorderableWidget", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { groupId, rect });
            }
            catch
            {
                disabled = true;
            }
        }

        public static void Resolve()
        {
            if (!Mod.ModNumbersActive)
            {
                disabled = true;
            }

            try
            {
                numbersType = GenTypes.GetTypeInAnyAssembly("Numbers.Numbers");
                pawnTableType = GenTypes.GetTypeInAnyAssembly("Numbers.PawnTable_NumbersMain");
                disabled = false;
            }
            catch
            {
                disabled = true;
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class ModPostInit
    {
        static ModPostInit()
        {
            if (Mod.Settings.firstRun)
            {
                Mod.Settings.firstRun = false;
                Mod.Settings.pawnTablesEnabled.AddRange(DefDatabase<CompatibilityInfoDef>.AllDefs.Where(x => x.compatibility == TableCompatibility.Compatible).SelectMany(x => x.tableNames));
            }
        }
    }

    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }
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
