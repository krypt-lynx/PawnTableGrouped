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
        public bool groupedAnimalsTab = true;
        public bool groupedWildlifeTab = true;
        public bool hideHeaderIfOnlyOneGroup = true;
        
        public bool interactiveGroupHeaderExperimental = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref groupedAnimalsTab, "groupedAnimalsTab", true);
            Scribe_Values.Look(ref groupedWildlifeTab, "groupedWildlifeTab", true);
            Scribe_Values.Look(ref hideHeaderIfOnlyOneGroup, "hideHeaderIfOnlyOneGroup", false);
            Scribe_Values.Look(ref interactiveGroupHeaderExperimental, "interactiveGroupHeaderExperimental", false);

            base.ExposeData();
        }
    }


    public class Mod : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }
        public static string CommitInfo = null;
        public static Verse.Mod Instance = null;

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
            CElement hideHeader;
            CElement interactiveHeader;
            CListView tablesList;
            CElement footer;
            CFrame listFrame;

            Gui.StackTop(
             (Gui.AddElement(hideHeader = new CCheckBox
             {
                 Title = "HideHeaderIfOnlyOneGroup".Translate(),
                 Checked = Settings.hideHeaderIfOnlyOneGroup,
                 Changed = (_, value) => Settings.hideHeaderIfOnlyOneGroup = value,
             }), hideHeader.intrinsicHeight),             
             10,
             (Gui.AddElement(interactiveHeader = new CCheckBox
             {
                 Title = "InteractiveGroupHeader".Translate(),
                 Checked = Settings.interactiveGroupHeaderExperimental,
                 Changed = (_, value) => Settings.interactiveGroupHeaderExperimental = value,
             }), interactiveHeader.intrinsicHeight),
             10,
             Gui.AddElement(listFrame = new CFrame()),
             10,
             (Gui.AddElement(footer = new CLabel
             {
                 Title = $"Grouped Pawns Lists version: {CommitInfo}",
                 TextAlignment = TextAnchor.LowerRight,
                 Color = new Color(1, 1, 1, 0.5f),
                 Font = GameFont.Tiny
             }), footer.intrinsicHeight)
            );

            tablesList = listFrame.AddElement(new CListView());
            listFrame.Embed(tablesList, new EdgeInsets(3));

            PopulateTablesList(tablesList);
        }

        private void PopulateTablesList(CListView tablesList)
        {
            var tables = DefDatabase<PawnTableDef>.AllDefs;
            var supported = new HashSet<string>(DefDatabase<StringListDef>.GetNamed("SupportedTables").list);
            var incompatible = new HashSet<string>(DefDatabase<StringListDef>.GetNamed("IncompatibleTables").list);
            int index = 0;

            foreach (var table in tables)
            {
                var row = new AlternatingBGRow();
                row.IsOdd = index % 2 == 1;
               /* var label = row.AddElement(new CLabel
                {
                    Title = table.defName,
                    Color = supported.Contains(table.defName) ? Color.green : (incompatible.Contains(table.defName) ? Color.red : Color.white)
                });*/
                
                var label = row.AddElement(new TablesListCheckbox
                {
                    Title = table.defName,
                    Color = supported.Contains(table.defName) ? Color.green : (incompatible.Contains(table.defName) ? Color.red : Color.white),
                });
                
                row.Embed(label, new EdgeInsets(0, 2, 0, 2));
                label.AddConstraint(label.height ^ label.intrinsicHeight);

                tablesList.AppendRow(row);
                index++;
            }
        }
    }


}
