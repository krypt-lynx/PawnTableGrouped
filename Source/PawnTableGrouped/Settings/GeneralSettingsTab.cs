﻿using RWLayout.alpha2;
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

            CreateInfoColumns();

            CElement modReport = new CElement();

            tabFrame.StackTop(StackOptions.Create(insets: new EdgeInsets(7)),
                (AddElement(debug = new CCheckboxLabeled
                {
                    Title = "DebugOutput".Translate(),
                    Checked = Mod.Settings.debug,
                    Changed = (_, value) => Mod.Settings.debug = value,
                }), debug.intrinsicHeight),
                2,
                (AddElement(hideHeader = new CCheckboxLabeled
                {
                    Title = "HideHeaderIfOnlyOneGroup".Translate(),
                    Checked = Mod.Settings.hideHeaderIfOnlyOneGroup,
                    Changed = (_, value) => Mod.Settings.hideHeaderIfOnlyOneGroup = value,
                }), hideHeader.intrinsicHeight),
                2,
                (AddElement(primarySort = new CCheckboxLabeled
                {
                    Title = "UsePrimarySortFunction".Translate(),
                    Checked = Mod.Settings.usePrimarySortFunction,
                    Changed = (_, value) => Mod.Settings.usePrimarySortFunction = value,
                }), hideHeader.intrinsicHeight),
                2,
                (AddElement(byColumn = new CCheckboxLabeled
                {
                    Title = "GroupByColumnExperimental".Translate(),
                    Checked = Mod.Settings.groupByColumnExperimental,
                    Changed = (_, value) => Mod.Settings.groupByColumnExperimental = value,
                }), byColumn.intrinsicHeight),
                AddElement(new CElement()), // flexable spacer
                AddElement(modReport)
            );


            var elements = new List<CElement>();
            tmp = new CLabel { Title = "Mods report:" };
            tmp.AddConstraint(tmp.height ^ tmp.intrinsicHeight);

            elements.Add(modReport.AddElement(tmp));
            elements.AddRange(Mod.ModBridges.Select(x => ConstructModStatusView(x.bridge)));

            modReport.StackTop(elements.ToArray<object>());

        }
        CVarListGuide Columns;

        private void CreateInfoColumns()
        {
            Columns = new CVarListGuide();
            for (int i = 0; i < 5; i++)
            {
                Columns.Variables.Add(new Cassowary.ClVariable($"column{i}"));
            }
            this.AddGuide(Columns);
        }

        string BoolToString(bool value, Color if_, Color else_)
        {
            return value ? "Yes".Colorize(if_) : "No".Colorize(else_);
        }

        CElement ConstructModStatusView(ModBridge bridge)
        {
            List<CElement> elements = new List<CElement>();


            elements.Add(AddElement(new CLabel
            {
                Title = bridge.ModName() + ": "
            }));
            elements.Add(AddElement(new CLabel
            {
                Title = "detected: "
            }));
            elements.Add(AddElement(new CLabel
            {
                Title = BoolToString(bridge.IsDetected, Color.green, Color.white) + "; "
            }));

            if (bridge.IsDetected)
            {
                elements.Add(AddElement(new CLabel
                {
                    Title = "interacting: "
                }));
                elements.Add(AddElement(new CLabel
                {
                    Title = BoolToString(bridge.IsActive, Color.green, Color.red)
                }));
            }

            CElement panel = this.AddElement(new CElement());
            panel.StackLeft(StackOptions.Create(constrainEnd: false), elements.ToArray<object>());
            for (int i = 0; i < elements.Count; i++)
            {
                panel.AddConstraints(
                    elements[i].width >= elements[i].intrinsicWidth,
                    elements[i].width <= Columns.Variables[i]);
            }

            panel.AddConstraint(
                elements[0].height ^ elements[0].intrinsicHeight
                );
            //);

            return panel;
        }
    }
}
