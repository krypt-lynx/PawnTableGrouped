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
    class TablesTab : CTabPage
    {
        TablesSettingsViewModel tablesModel;

        protected override void ConstructGUI()
        {
            base.ConstructGUI();

            tablesModel = new TablesSettingsViewModel();
            ConstructTablesTab();
        }


        private void ConstructTablesTab()
        {
            CElement listFrame;
            CElement actionsGroup;

            this.AddElement(listFrame = new CFrame());
            this.Embed(listFrame);
            actionsGroup = listFrame.AddElement(new CElement());
            actionsGroup.StackLeft(
                2,
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

            var tablesList = listFrame.AddElement(new CListView());
            listFrame.StackTop(StackOptions.Create(insets: new EdgeInsets(3)),
                (actionsGroup, 30),
                10,
                tablesList);


            PopulateTablesList(tablesList);
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
                    (column0, row.width * 0.45), 2, column1);

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

    class ColumnsTab : CTabPage, IListViewDataSource
    {
        List<PawnColumnDef> columnDefs;
        float rowHeight;
        CListingRow firstRow;
        public ColumnsTab()
        {
            columnDefs = DefDatabase<GroupColumnWorkerDef>.AllDefs.Select(x => x.Worker.ColumnDef).Concat(
          DefDatabase<PawnColumnDef>.AllDefs.Where(x => GroupColumnDefResolver.GetResolver(x, false) == null)
            ).ToList();
            if (columnDefs.Count > 0) // this is a hack to resolve row height using constraints
            {
                firstRow = ListingRowForRowAt(0);
                firstRow.InRect = new Rect(0, 0, 100, 0);
                firstRow.UpdateLayoutIfNeeded();

                rowHeight = firstRow.Bounds.height;
            }
        }

        public float HeightForRowAt(int index)
        {
            return rowHeight;
        }

        public CListingRow ListingRowForRowAt(int index)
        {
            if (index == 0 && firstRow != null)
            {
                return firstRow;
            }

            var column = columnDefs[index];
            var resolver = GroupColumnDefResolver.GetResolver(column, false);

            var row = new AlternatingBGRow();

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

            return row;
        }

        public int NumberOfRows()
        {
            return columnDefs.Count();
        }

        protected override void ConstructGUI()
        {
            base.ConstructGUI();
            ConstructColumnsTab();
        }

        private void ConstructColumnsTab()
        {
            CElement listFrame;

            this.AddElement(listFrame = new CFrame());
            this.Embed(listFrame);

            var columnsList = listFrame.AddElement(new CListView_vNext());
            listFrame.Embed(columnsList, new EdgeInsets(3));
            columnsList.DataSource = this;
        }
    }

    class SettingsView : CElement
    {


        public SettingsView() : base()
        {
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


            tabs.AddTab(new TablesTab
            {
                Title = "Tables"
            });

            tabs.AddTab(new ColumnsTab
            {
                Title = "Columns"
            });

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

    }

}
