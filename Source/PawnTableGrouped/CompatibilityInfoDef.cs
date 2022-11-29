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
        public Type settingsWorker = null;
        public TableConfig config;
    }

    public class TableConfig
    {
        public bool allowHScroll = false;
        public float expandedBottomSpace = 0;
        public float footerBtnOffset = 0;
    }

    public class CompatibilityInfoDef : Def
    {
        public string packageId;
        public string modName;
        public int priority;
        public List<TableInfo> tables;

        [Unsaved(false)]
        private static Dictionary<string, TableInfo> currentTables;
        public static Dictionary<string, TableInfo> CurrentTables
        {
            get
            {
                if (currentTables == null)
                {
                    var loadedModIds = Mod.RunningModInvariantIds.ToHashSet();

                    currentTables = new Dictionary<string, TableInfo>();

                    foreach (var tableInfo in DefDatabase<CompatibilityInfoDef>.AllDefs.Where(x => loadedModIds.Contains(x.packageId.ToLowerInvariant())).OrderBy(x => x.priority).SelectMany(x => x.tables))
                    {
                        currentTables[tableInfo.name] = tableInfo;
                    }
                }
                return currentTables;
            }
        }
    }

    public enum TableCompatibility
    {
        Incompatible,
        Issues,
        Compatible,
        Supported,
    }

}
