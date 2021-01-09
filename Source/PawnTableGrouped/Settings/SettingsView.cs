using Cassowary;
using RimWorld;
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
    class SettingsView : CElement
    {
        TablesSettingsViewModel tablesModel;

        CVarListGuide columnGuide;
        CListView tablesList;
        ClVariable column0;

        public SettingsView() : base()
        {
            tablesModel = new TablesSettingsViewModel();

            CElement debug;
            CElement hideHeader;
            CElement primarySort;
            CElement byColumn;
            CElement footer;
            CElement listTitle;
            CElement listFrame;
            CElement actionsGroup;

            this.StackTop(
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
                }), byColumn.intrinsicHeight),
                10,
                (AddElement(listTitle = new CLabel {                    
                    Title = "EnableGroupingInTables".Translate(),
                }), listTitle.intrinsicHeight),
                2,
                AddElement(listFrame = new CFrame()),
                7
            );

            actionsGroup = listFrame.AddElement(new CElement());
            actionsGroup.StackLeft(
                actionsGroup.AddElement(new CLabel
                {
                    Title = "PTG_Select".Translate(),
                    TextAlignment = TextAnchor.MiddleLeft,
                }),
                10,
                (actionsGroup.AddElement(new CButtonText
                {
                    Title = "PTG_None".Translate(),
                    Action = (_) => tablesModel.SelectNone(),
                }), 200),
                10,
                (actionsGroup.AddElement(new CButtonText
                {
                    Title = "PTG_AllSupported".Translate(),
                    Action = (_) => tablesModel.SelectAllAtLeast(TableCompatibility.Supported),
                }), 200),
                10,
                (actionsGroup.AddElement(new CButtonText
                {
                    Title = "PTG_AllCompatible".Translate(),
                    Action = (_) => tablesModel.SelectAllAtLeast(TableCompatibility.Issues),
                }), 200)
                );

            tablesList = listFrame.AddElement(new CListView());
            listFrame.StackTop(StackOptions.Create(insets: new EdgeInsets(3)), 
                (actionsGroup, 30),
                10, 
                tablesList);

            columnGuide = new CVarListGuide();
            column0 = new Cassowary.ClVariable("column0");
            columnGuide.Variables.Add(column0);

            tablesList.AddGuide(columnGuide);
            tablesList.AddConstraint(column0 ^ tablesList.width * 0.45);

            footer = AddElement(new CLabel
            {
                Title = $"Grouped Pawns Lists version: {Mod.CommitInfo}",
                TextAlignment = TextAnchor.LowerRight,
                Color = new Color(1, 1, 1, 0.5f),
                Font = GameFont.Tiny
            });

            this.AddConstraints(
                footer.top ^ this.bottom + 3,
                footer.width ^ footer.intrinsicWidth,
                footer.right ^ this.right,
                footer.height ^ footer.intrinsicHeight);

            PopulateTablesList(tablesList);
        }

        public override void PostLayoutUpdate()
        {
            foreach (var row in tablesList.Rows)
            {
                var guide = (ColumnGuide)row.Guides[0];
                guide.UpdateColumnWidth(0, column0.Value);
            }

            base.PostLayoutUpdate();
        }

        Color colorForPawnTable(TablesSettingsViewModel.TableData tableData)
        {
            switch (tableData.compatibility)
            {
                case TableCompatibility.Supported:
                    return Color.green;
                case TableCompatibility.Compatible:
                    return Color.white;
                case TableCompatibility.Issues:
                    return Color.yellow;
                case TableCompatibility.Incompatible:
                    return Color.red;
                default:
                    return Color.white;
            }
        }

        private void PopulateTablesList(CListView tablesList)
        {       
            int index = 0;

            foreach (var table in tablesModel.Tables)
            {
                var row = new AlternatingBGRow();
                row.AddGuide(new ColumnGuide(1));

                row.IsOdd = index % 2 == 1;

                var column0 = row.AddElement(new CElement
                {
                    Tip = table.tip,
                });

                var checkbox = row.AddElement(new CCheckbox
                {
                    Checked = table.selected,
                    Changed = (_, value) => tablesModel.SetSelected(table, value),
                    Paintable = true,
                });
                var title = row.AddElement(new CLabel
                {
                    Title = table.defName,
                    Color = colorForPawnTable(table),
                });

                var column1 = row.AddElement(new CElement
                {
                    Tip = table.packageId,
                });

                var description = column1.AddElement(new CLabel
                {
                    Title = table.modName,
                });

                column0.StackLeft(StackOptions.Create(constrainSides: false), 2, checkbox, 2, title);
                title.AddConstraint(title.height ^ title.intrinsicHeight);

                checkbox.MakeSizeIntristic();
                column0.AddConstraint(column0.centerY ^ checkbox.centerY);
                column0.AddConstraints(column0.top ^ title.top, column0.bottom ^ title.bottom);


                column1.Embed(description, new EdgeInsets(0, 2, 0, 2));
                column1.AddConstraint(description.height ^ description.intrinsicHeight);

                row.StackLeft(StackOptions.Create(constrainSides: false),
                    (column0, ((ColumnGuide)row.Guides[0]).columns[0]), 2, column1);

                row.AddConstraints(
                    column0.top ^ row.top, column0.bottom <= row.bottom,
                    column1.top ^ row.top, column1.bottom <= row.bottom);

                tablesList.AppendRow(row);

                table.OnChanged = (t) =>
                {
                    checkbox.Checked = t.selected;
                };

                index++;
            }
        }
    }

}
