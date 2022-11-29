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
#if rw_1_4_or_later
        static bool fixWorkTabDefault = true;
#else
        static bool fixWorkTabDefault = false;
#endif

        public bool firstRun = true;
        public bool debug = false;
        public bool showDummyColumns = false;
        public bool hideHeaderIfOnlyOneGroup = false;
        public bool usePrimarySortFunction = true;
        public bool disableGroupCells = false;
        public bool groupByColumnExperimental = false;
        public bool fixWorkTab = fixWorkTabDefault;

        public HashSet<string> pawnTablesEnabled = new HashSet<string>();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref firstRun, "firstRun2", true);

            Scribe_Values.Look(ref debug, nameof(debug), false);
            Scribe_Values.Look(ref showDummyColumns, nameof(showDummyColumns), false);
            Scribe_Values.Look(ref hideHeaderIfOnlyOneGroup, nameof(hideHeaderIfOnlyOneGroup), false);
            Scribe_Values.Look(ref usePrimarySortFunction, nameof(usePrimarySortFunction), true);
            Scribe_Values.Look(ref disableGroupCells, nameof(disableGroupCells), false);
            Scribe_Values.Look(ref groupByColumnExperimental, nameof(groupByColumnExperimental), false);
            Scribe_Values.Look(ref fixWorkTab, nameof(fixWorkTab), fixWorkTabDefault);

            Scribe_Collections.Look(ref pawnTablesEnabled, "pawnTablesEnabled");

            pawnTablesEnabled ??= new HashSet<string>();

            base.ExposeData();
        }
    }

}
