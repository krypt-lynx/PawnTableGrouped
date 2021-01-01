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


    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }
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
