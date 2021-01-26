using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    [StaticConstructorOnStartup]
    public static class ModPostInit
    {
        static ModPostInit()
        {
            Mod.DetectMods();
            // enabling supported tables if first run
            if (Mod.Settings.firstRun)
            {
                Mod.Settings.firstRun = false;

                Mod.Settings.pawnTablesEnabled.AddRange(
                    CompatibilityInfoDef.CurrentTables.Where(kvp => kvp.Value.compatibility == TableCompatibility.Supported).Select(kvp => kvp.Key)
                    );
            }
        }
    }

}
