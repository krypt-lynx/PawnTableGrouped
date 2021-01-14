using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class TableInfo
    {
        public string name;
        public TableCompatibility compatibility;
        public string defaultGrouping;
        public string hint;
        public string issues;
        public TableConfig config;
    }

    public class TableConfig
    {
        public bool allowHScroll = false;
        public float expandedBottomSpace = 0;
        public float fotterBtnOffset = 0;
    }

    public class CompatibilityInfoDef : Def
    {
        public List<ModCompatibility> compatibilityList;

        [Unsaved(false)]
        private static Dictionary<string, TableInfo> currentTables;
        public static Dictionary<string, TableInfo> CurrentTables
        {
            get
            {
                if (currentTables == null)
                {
                    var info = DefDatabase<CompatibilityInfoDef>.GetNamed("ModCompatibility");
                    var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageId).ToHashSet();

                    currentTables = new Dictionary<string, TableInfo>();

                    foreach (var tableInfo in info.compatibilityList.Where(x => loadedModIds.Contains(x.packageId)).SelectMany(x => x.tables))
                    {
                        currentTables[tableInfo.name] = tableInfo;
                    }
                }
                return currentTables;
            }
        }
    }

    public class ModCompatibility
    {
        public string packageId;
        public string modName;
        public List<TableInfo> tables;
    }

    public enum TableCompatibility
    {
        Incompatible,
        Issues,
        Compatible,
        Supported,
    }

}
