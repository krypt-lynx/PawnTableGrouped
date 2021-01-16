using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    class GeneralSettingsTab : CTabPage
    {
        public override string Title()
        {
            return "PTG_General".Translate();
        }

        protected override void ConstructGUI()
        {
            CElement tabFrame = this.AddElement(new CFrame());
            this.Embed(tabFrame);


            CElement debug;
            CElement hideHeader;
            CElement primarySort;
            CElement byColumn;


            tabFrame.StackTop(StackOptions.Create(constrainEnd:false, insets:new EdgeInsets(5)),
                2,
                (AddElement(debug = new CCheckboxLabeled
                {
                    Title = "DebugOutput".Translate(),
                    Checked = Mod.Settings.hideHeaderIfOnlyOneGroup,
                    Changed = (_, value) => Mod.Settings.hideHeaderIfOnlyOneGroup = value,
                }), debug.intrinsicHeight),
                10,
                (AddElement(hideHeader = new CCheckboxLabeled
                {
                    Title = "HideHeaderIfOnlyOneGroup".Translate(),
                    Checked = Mod.Settings.hideHeaderIfOnlyOneGroup,
                    Changed = (_, value) => Mod.Settings.hideHeaderIfOnlyOneGroup = value,
                }), hideHeader.intrinsicHeight),
                10,
                (AddElement(primarySort = new CCheckboxLabeled
                {
                    Title = "UsePrimarySortFunction".Translate(),
                    Checked = Mod.Settings.usePrimarySortFunction,
                    Changed = (_, value) => Mod.Settings.usePrimarySortFunction = value,
                }), hideHeader.intrinsicHeight),
                10,
                (AddElement(byColumn = new CCheckboxLabeled
                {
                    Title = "GroupByColumnExperimental".Translate(),
                    Checked = Mod.Settings.groupByColumnExperimental,
                    Changed = (_, value) => Mod.Settings.groupByColumnExperimental = value,
                }), byColumn.intrinsicHeight)
            );
        }
    }
}
