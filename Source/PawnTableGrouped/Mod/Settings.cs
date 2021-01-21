using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class Settings : ModSettings
    {
        public bool firstRun = true;
        public bool debug = false;
        public bool hideHeaderIfOnlyOneGroup = false;
        public bool usePrimarySortFunction = true;
        public bool groupByColumnExperimental = false;

        public HashSet<string> pawnTablesEnabled = new HashSet<string>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref firstRun, "firstRun2", true);

            Scribe_Values.Look(ref debug, "debug", false);
            Scribe_Values.Look(ref hideHeaderIfOnlyOneGroup, "hideHeaderIfOnlyOneGroup", false);
            Scribe_Values.Look(ref usePrimarySortFunction, "usePrimarySortFunction", true);
            Scribe_Values.Look(ref groupByColumnExperimental, "groupByColumnExperimental", false);

            Scribe_Collections.Look(ref pawnTablesEnabled, "pawnTablesEnabled");

            pawnTablesEnabled ??= new HashSet<string>();

            base.ExposeData();
        }
    }

}
