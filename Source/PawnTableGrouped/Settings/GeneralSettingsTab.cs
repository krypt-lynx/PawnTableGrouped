using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
            CElement tmp; // lets hope I will not hit undefined behavior 

            tabFrame.StackTop(StackOptions.Create(insets:new EdgeInsets(7)),
                (AddElement(debug = new CCheckboxLabeled
                {
                    Title = "DebugOutput".Translate(),
                    Checked = Mod.Settings.debug,
                    Changed = (_, value) => Mod.Settings.debug = value,
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
                }), byColumn.intrinsicHeight),
                AddElement(new CElement()), // flexable spacer
                (AddElement(tmp = new CLabel
                {
                    Title = ModInfoString("Numbers", NumbersBridge.Instance),
                }), tmp.intrinsicHeight),
                (AddElement(tmp = new CLabel
                {
                    Title = ModInfoString("Simple Slavery", SimpleSlaveryBridge.Instance),
                }), tmp.intrinsicHeight),
                (AddElement(tmp = new CLabel
                {
                    Title = ModInfoString("Work Tab", WorkTabBridge.Instance),
                }), tmp.intrinsicHeight)
            );
        }

        string ModInfoString<T>(string modName, ModBridge<T> bridge) where T : ModBridge<T>
        {
            StringBuilder sb = new StringBuilder();

            string BoolToString(bool value, Color if_, Color else_)
            {
                return value ? "Yes".Colorize(if_) : "No".Colorize(else_);
            }

            sb.Append($"{modName}: detected: {BoolToString(bridge.IsDetected, Color.green, Color.white)};");
            if (bridge.IsDetected)
            {
                sb.Append($" interacting: { BoolToString(bridge.IsActive, Color.green, Color.red)};");
            }

            return sb.ToString();
        }
    }
}
