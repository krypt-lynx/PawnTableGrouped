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
    class TablesSettingsTab : CTabPage
    {
        TablesSettingsViewModel tablesModel;

        public override string Title()
        {
            return "Tables".Translate();
        }

        protected override void ConstructGUI()
        {
            base.ConstructGUI();

            tablesModel = new TablesSettingsViewModel();
            ConstructTablesTab();
        }


        private void ConstructTablesTab()
        {
            CElement tabFrame = this.AddElement(new CFrame());
            this.Embed(tabFrame);

            CElement actionsGroup = tabFrame.AddElement(new CElement());
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

            var tablesList = tabFrame.AddElement(new CListView());
            tabFrame.StackTop(StackOptions.Create(insets: new EdgeInsets(3)),
                (actionsGroup, 30),
                10,
                tablesList);


            PopulateTablesList(tablesList);
        }

        Color colorForPawnTable(TablesSettingsViewModel.TableData tableData)
        {
            switch (tableData.Compatibility)
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
                    Tip = table.Tip,
                });

                var checkbox = row.AddElement(new CCheckbox
                {
                    Checked = table.Selected,
                    Changed = (_, value) => tablesModel.SetSelected(table, value),
                    Paintable = true,
                });
                var title = row.AddElement(new CLabel
                {
                    Title = table.DefName,
                    Color = colorForPawnTable(table),
                });

                var column1 = row.AddElement(new CElement
                {
                    Tip = table.PackageId,
                });

                var description = column1.AddElement(new CLabel
                {
                    Title = table.ModName,
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
                    checkbox.Checked = t.Selected;
                };

                index++;
            }
        }

    }

}
