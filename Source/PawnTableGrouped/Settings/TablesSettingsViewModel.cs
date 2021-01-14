using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
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


            var info = DefDatabase<CompatibilityInfoDef>.GetNamed("ModCompatibility");


            foreach (var modInfo in info.compatibilityList)
            {
                if (loadedModIds.Contains(modInfo.packageId))
                {
                    foreach (var tableInfo in modInfo.tables)
                    {
                        TableData data = null;

                        if (defToTable.TryGetValue(tableInfo.name, out data))
                        {
                            data.modName = modInfo.modName;
                            data.packageId = modInfo.packageId;
                            data.compatibility = tableInfo.compatibility;
                            data.tip = tableInfo.hint;
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
