﻿using Cassowary;
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

        ClVariable tablesColumn0; // todo: propers columns implementation
        CListView tablesList;


        CListView columnsList;

        public SettingsView() : base()
        {
            tablesModel = new TablesSettingsViewModel();

            CElement debug;
            CElement hideHeader;
            CElement primarySort;
            CElement byColumn;
            CTabsView tabs;


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
                AddElement(tabs = new CTabsView()),
                7
            );

            var tablesTab = new CTabPage
            {
                Title = "Tables"
            };
            tabs.AddTab(tablesTab);
            ConstructTablesTab(tablesTab);

            var columnsTab = new CTabPage
            {
                Title = "Columns"
            };
            tabs.AddTab(columnsTab);
            ConstructColumnsTab(columnsTab);

            CElement footer;

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

        }

        private void ConstructTablesTab(CTabPage tablesTab)
        {
            CElement listFrame;
            CElement actionsGroup;

            tablesTab.AddElement(listFrame = new CFrame());
            tablesTab.Embed(listFrame);
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

            var columnGuide = new CVarListGuide();
            tablesColumn0 = new Cassowary.ClVariable("tColumn0");
            columnGuide.Variables.Add(tablesColumn0);

            tablesList.AddGuide(columnGuide);
            tablesList.AddConstraint(tablesColumn0 ^ tablesList.width * 0.45);

            PopulateTablesList(tablesList);
        }
  
        private void ConstructColumnsTab(CTabPage tablesTab)
        {
            CElement listFrame;

            tablesTab.AddElement(listFrame = new CFrame());
            tablesTab.Embed(listFrame);

            columnsList = listFrame.AddElement(new CListView());
            listFrame.Embed(columnsList, new EdgeInsets(3));


            PopulateColumnsList(columnsList);
        }

        public override void PostLayoutUpdate()
        {
            foreach (var row in tablesList.Rows)
            {
                var guide = (ColumnGuide)row.Guides[0];
                guide.UpdateColumnWidth(0, tablesColumn0.Value);
            }

            /*
            foreach (var row in columnsList.Rows)
            {
                var guide = (ColumnGuide)row.Guides[0];
                guide.UpdateColumnWidth(0, columnsColumn0.Value);
                guide.UpdateColumnWidth(1, columnsColumn1.Value);
            }*/
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

        private void PopulateColumnsList(CListView columnsList)
        {
            var resolvers = DefDatabase<GroupColumnWorkerDef>.AllDefs;
            /*
            $"Generated: column: {column.defName}; worker {column.workerClass.FullName} => {resolverDef.workerClass.FullName}".Log();
            */

            int index = 0;

            var columns =
                DefDatabase<GroupColumnWorkerDef>.AllDefs.Select(x => x.Worker.ColumnDef).Concat(
                    DefDatabase<PawnColumnDef>.AllDefs.Where(x => GroupColumnDefResolver.GetResolver(x, false) == null)
                );

            foreach (var column in columns)
            {
                var resolver = GroupColumnDefResolver.GetResolver(column, false);

                var row = new AlternatingBGRow();
                var columnsGuide = new ColumnGuide(3);
                row.AddGuide(columnsGuide);

                row.IsOdd = index % 2 == 1;

                var column0 = row.AddElement(new CLabel
                {
                    Title = column.defName,
                    WordWrap = true,
                });
                var column1 = row.AddElement(new CLabel
                {
                    Title = column.workerClass.FullName,
                });
                var column2 = row.AddElement(new CLabel
                {
                    Color = resolver != null ? Color.white : new Color(1, 1, 1, 0.4f),
                    Title = resolver != null ? resolver.workerClass?.FullName : "<unresolved>",
                });

                /*
                row.AddConstraints( // yeah, I have no horisontal scroll view in CListView implemented
                    column0.top ^ row.top + 2,
                    column1.top ^ column0.bottom + 2,
                    column2.top ^ column1.bottom + 2,
                    row.bottom ^ column2.bottom + 2,

                    row.left ^ column0.left, column0.right + 40 ^ row.right,
                    row.left + 20 ^ column1.left, column1.right + 20 ^ row.right,
                    row.left + 40 ^ column2.left, column2.right ^ row.right,

                    column0.height ^ column0.intrinsicHeight,
                    column1.height ^ column1.intrinsicHeight,
                    column2.height ^ column2.intrinsicHeight
                    );
                */

                row.AddConstraints(
                    column0.top ^ row.top + 2,
                    row.bottom ^ column0.bottom + 2,

                    column1.top ^ row.top + 2,
                    column2.top ^ column1.bottom + 2,
                    row.bottom ^ column2.bottom + 2,

                    column0.left ^ row.left + 2, 
                    column1.left ^ column0.right + 2,
                    column2.left ^ column0.right + 2,

                    row.right ^ column1.right + 2,
                    row.right ^ column2.right + 2,

                    column1.height ^ column1.intrinsicHeight,
                    column2.height ^ column2.intrinsicHeight,

                    column0.width ^ row.width * 0.33
                    );

                /*
                row.StackTop(StackOptions.Create(insets: new EdgeInsets(2)), (column0, column0.intrinsicHeight), 2, (column1, column1.intrinsicHeight), 2, (column2, column2.intrinsicHeight));
                */
                columnsList.AppendRow(row);


                index++;
            }
        }
    }

}
