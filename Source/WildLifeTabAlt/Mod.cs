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

namespace WildlifeTabAlt
{
    public class Settings : ModSettings
    {
        public bool groupedAnimalsTab = true;
        public bool groupedWildlifeTab = true;
        
        public bool interactiveGroupHeaderExperimental = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref groupedAnimalsTab, "groupedAnimalsTab", true);
            Scribe_Values.Look(ref groupedWildlifeTab, "groupedWildlifeTab", true);
            Scribe_Values.Look(ref interactiveGroupHeaderExperimental, "interactiveGroupHeaderExperimental", false);

            base.ExposeData();
        }
    }


    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }
        public static string CommitInfo = null;
        public static ModContentPack Content = null;

        public Mod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            Settings = GetSettings<Settings>();
            Content = content;

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
            return "Wildlife Tab Alt";
        }

        public override void ConstructGui()
        {
            Gui.StackTop(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false),
             Gui.AddElement(new CCheckBox
             {
                 Title = "GroupedAnimalsTab".Translate(),
                 Checked = Settings.groupedAnimalsTab,
                 Changed = (_, value) => Settings.groupedAnimalsTab = value,
             }),
             2,
             Gui.AddElement(new CCheckBox
             {
                 Title = "GroupedWildlifeTab".Translate(),
                 Checked = Settings.groupedWildlifeTab,
                 Changed = (_, value) => Settings.groupedWildlifeTab = value,
             }),
             10, 
             Gui.AddElement(new CCheckBox
             {
                 Title = "InteractiveGroupHeader".Translate(),
                 Checked = Settings.interactiveGroupHeaderExperimental,
                 Changed = (_, value) => Settings.interactiveGroupHeaderExperimental = value,
             }));

            Gui.StackBottom(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false),
                Gui.AddElement(new CLabel
                {
                    Title = $"WildlifeTabAlt version: {CommitInfo}",
                    TextAlignment = TextAnchor.LowerRight,
                    Color = new Color(1, 1, 1, 0.5f),
                    Font = GameFont.Tiny
                })
            );
        }
    }

}
