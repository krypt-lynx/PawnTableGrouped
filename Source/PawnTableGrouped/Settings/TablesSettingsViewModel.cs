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
            public TableCompatibility Compatibility;
            public string DefName;
            public string ModName;
            public string PackageId;
            public bool Selected;
            public string Tip;
            public Action<TableData> OnChanged;
            public ISettingsWorker Worker;

            public void NotifySettingsWorker()
            {
                Worker?.TableActiveChanged(DefName, Selected);
            }

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
                    DefName = def.defName,
                    Compatibility = TableCompatibility.Compatible,
                    Selected = Mod.Settings.pawnTablesEnabled.Contains(def.defName),
                }).ToList();

            defToTable = Tables.ToDictionary(d => d.DefName, d => d);

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
                            data.ModName = modInfo.modName;
                            data.PackageId = modInfo.packageId;
                            data.Compatibility = tableInfo.compatibility;
                            data.Tip = tableInfo.hint;
                            data.Worker = tableInfo.settingsWorker != null ? (ISettingsWorker)Activator.CreateInstance(tableInfo.settingsWorker) : null;
                        }
                    }
                }
            }
        }

        public void SetSelected(TableData table, bool value)
        {
            table.Selected = value;
            if (value)
            {
                Mod.Settings.pawnTablesEnabled.Add(table.DefName);
            }
            else
            {
                Mod.Settings.pawnTablesEnabled.Remove(table.DefName);
            }
            table.NotifySettingsWorker(); 
            Mod.DoActiveTablesChanged();
        }

        public void SelectAllAtLeast(TableCompatibility compatibility)
        {
            Mod.Settings.pawnTablesEnabled.Clear();

            foreach (var table in Tables)
            {
                if (table.Compatibility >= compatibility)
                {
                    Mod.Settings.pawnTablesEnabled.Add(table.DefName);
                    table.Selected = true;
                }
                else
                {
                    table.Selected = false;
                }
                table.NotifySettingsWorker();
                table.DoChanged();
            }

            Mod.DoActiveTablesChanged();
        }

        public void SelectNone()
        {
            Mod.Settings.pawnTablesEnabled.Clear();

            foreach (var table in Tables)
            {
                table.Selected = false;
                table.NotifySettingsWorker();
                table.DoChanged();
            }

            Mod.DoActiveTablesChanged();
        }
    }

}
