using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class CompatibilityInfoDef : Def
    {
        public string packageId;
        public string modName;
        public List<string> tableNames;
        public string hint;
        public string issues;
        public TableCompatibility compatibility;
    }

    public enum TableCompatibility
    {
        Incompatible,
        Compatible,
        Supported,
    }

    public class TablesSettingsViewModel
    {
        public class TableData
        {
            public TableCompatibility compatibility;
            public string defName;
            public string modName;
            public string packageId;
            public bool selected;
            public string tip;
            public Action<TableData> OnChanged;

            public void DoChanged()
            {
                OnChanged?.Invoke(this);
            }
        }

        public List<TableData> Tables;
        public Dictionary<string, TableData> defToTable;


        public TablesSettingsViewModel()
        {
            Tables = DefDatabase<PawnTableDef>.AllDefs
                .Select(def => new TableData
                {
                    defName = def.defName,
                    compatibility = TableCompatibility.Compatible,
                    selected = Mod.Settings.pawnTablesEnabled.Contains(def.defName),
                }).ToList();

            defToTable = Tables.ToDictionary(d => d.defName, d => d);

            DetectMods();

        }

        private void DetectMods()
        {
            var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageId).ToHashSet();


            var info = DefDatabase<CompatibilityInfoDef>.AllDefs;


            foreach (var compatibility in info)
            {
                if (loadedModIds.Contains(compatibility.packageId))
                {
                    foreach (var tableName in compatibility.tableNames)
                    {
                        TableData data = null;
                        if (defToTable.TryGetValue(tableName, out data))
                        {
                            data.compatibility = compatibility.compatibility;
                            data.modName = compatibility.modName;
                            data.tip = compatibility.issues == null
                                ? compatibility.hint
                                : $"{compatibility.hint}\n\nIssues: {compatibility.issues}";
                            data.packageId = compatibility.packageId;
                        }
                    }
                }
            }
        }

        public void SetSelected(TableData table, bool value)
        {
            table.selected = value;
            if (value)
            {
                Mod.Settings.pawnTablesEnabled.Add(table.defName);
            }
            else
            {
                Mod.Settings.pawnTablesEnabled.Remove(table.defName);
            }
            Mod.DoActiveTablesChanged();
        }

        public void SelectAllAtLeast(TableCompatibility compatibility)
        {
            Mod.Settings.pawnTablesEnabled.Clear();

            foreach (var table in Tables)
            {
                if (table.compatibility >= compatibility)
                {
                    Mod.Settings.pawnTablesEnabled.Add(table.defName);
                    table.selected = true;
                }
                else
                {
                    table.selected = false;
                }
                table.DoChanged();
            }

            Mod.DoActiveTablesChanged();
        }

        public void SelectNone()
        {
            Mod.Settings.pawnTablesEnabled.Clear();

            foreach (var table in Tables)
            {
                table.selected = false;
                table.DoChanged();
            }

            Mod.DoActiveTablesChanged();
        }
    }

}
