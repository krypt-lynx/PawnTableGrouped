using Cassowary;
using RimWorld;
using RWLayout.alpha2;
using RWLayout.alpha2.Elements.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class CompatibilityInfoDef : Def
    {
        public string packageId;
        public string modName;
        public List<string> tableNames;
        public string hint;
        public string issues;
        public TableCompatibility compatibility;
    }

    public enum TableCompatibility
    {
        Compatible,
        Supported,
        Incompatible,
    }

    public class TablesSettingsViewModel
    {
        public class TableData
        {
            public TableCompatibility compatibility;
            public string defName;
            public string modName;
            public string packageId;
            public bool selected;
            public string tip;
            public Action<TableData> OnChanged;

            public void DoChanged()
            {
                OnChanged?.Invoke(this);
            }
        }

        public List<TableData> Tables;
        public Dictionary<string, TableData> defToTable;


        public TablesSettingsViewModel()
        {
            Tables = DefDatabase<PawnTableDef>.AllDefs
                .Select(def => new TableData
                {
                    defName = def.defName,
                    selected = Mod.Settings.pawnTablesEnabled.Contains(def.defName),
                }).ToList();

            defToTable = Tables.ToDictionary(d => d.defName, d => d);

            DetectMods();

        }

        private void DetectMods()
        {
            var loadedModIds = LoadedModManager.RunningMods.Select(x => x.PackageId).ToHashSet();


            var info = DefDatabase<CompatibilityInfoDef>.AllDefs;


            foreach (var compatibility in info)
            {
                if (loadedModIds.Contains(compatibility.packageId))
                {
                    foreach (var tableName in compatibility.tableNames)
                    {
                        TableData data = null;
                        if (defToTable.TryGetValue(tableName, out data))
                        {
                            data.compatibility = compatibility.compatibility;
                            data.modName = compatibility.modName;
                            data.tip = compatibility.issues == null
                                ? compatibility.hint 
                                : $"{compatibility.hint}\n\nIssues: {compatibility.issues}";
                            data.packageId = compatibility.packageId;
                        }
                    }
                }
            }
        }

        public void SetSelected(TableData table, bool value)
        {
            table.selected = value;
            if (value)
            {
                Mod.Settings.pawnTablesEnabled.Add(table.defName);
            } else
            {
                Mod.Settings.pawnTablesEnabled.Remove(table.defName);
            }
            Mod.DoActiveTablesChanged();
        }

        public void SelectAllAtLeast(TableCompatibility compatibility)
        {
            Mod.Settings.pawnTablesEnabled.Clear();

            foreach (var table in Tables)
            {
                if (table.compatibility >= compatibility)
                {
                    Mod.Settings.pawnTablesEnabled.Add(table.defName);
                }
                table.DoChanged();
            }

            Mod.DoActiveTablesChanged();
        }

        public void SelectNone()
        {
            Mod.Settings.pawnTablesEnabled.Clear();

            foreach (var table in Tables)
            {
                table.DoChanged();
            }

            Mod.DoActiveTablesChanged();
        }
    }

    public class ColumnGuide : CLayoutGuide
    {
        public ColumnGuide(int columnsCount)
        {
            columns_ = new Anchor[columnsCount];
            for (int i = 0; i < columnsCount; i++)
            {
                columns_[i] = new Anchor();
            }
        }

        private Anchor[] columns_; 

        private ClVariable[] columnsCached = null;
        public ClVariable[] columns
        {
            get
            {
                if (columnsCached == null)
                {
                    columnsCached = columns_.Select((c, i) => Parent.GetVariable(ref columns_[i], $"column{i}")).ToArray();
                }
                return columnsCached;
            }
        }

        public void UpdateColumnWidth(int index, double width)
        {
            Parent.UpdateStayConstrait(ref columns_[index], width);
        }

        public override void AddImpliedConstraints()
        {
            for (int i = 0; i < columns_.Length; i++)
            {
                Parent.CreateConstraintIfNeeded(ref columns_[i], () => new ClStayConstraint(columns[i]));
            }
        }

        public override void RemoveImpliedConstraints()
        {
            for (int i = 0; i < columns_.Length; i++)
            {
                Parent.RemoveVariableIfNeeded(ref columns_[i]);
            }
        }

        public override IEnumerable<ClVariable> enumerateAnchors()
        {
            return columns;
        }
    }


    class SettingsView : CElement
    {
        TablesSettingsViewModel tablesModel;

        CVarListGuide columnGuide;
        CListView tablesList;
        ClVariable column0;
        public SettingsView() : base()
        {
            tablesModel = new TablesSettingsViewModel();

            CElement hideHeader;
            CElement interactiveHeader;
            CElement footer;
            CElement actionsGroup;
            CFrame listFrame;

            this.StackTop(
                (AddElement(hideHeader = new CCheckboxLabeled
                {
                    Title = "HideHeaderIfOnlyOneGroup".Translate(),
                    Checked = Mod.Settings.hideHeaderIfOnlyOneGroup,
                    Changed = (_, value) => Mod.Settings.hideHeaderIfOnlyOneGroup = value,
                }), hideHeader.intrinsicHeight),
                10,
                (AddElement(interactiveHeader = new CCheckboxLabeled
                {
                    Title = "InteractiveGroupHeader".Translate(),
                    Checked = Mod.Settings.interactiveGroupHeader,
                    Changed = (_, value) => Mod.Settings.interactiveGroupHeader = value,
                }), interactiveHeader.intrinsicHeight),
                10,
                AddElement(listFrame = new CFrame()),
                10,
                (AddElement(footer = new CLabel
                {
                    Title = $"Grouped Pawns Lists version: {Mod.CommitInfo}",
                    TextAlignment = TextAnchor.LowerRight,
                    Color = new Color(1, 1, 1, 0.5f),
                    Font = GameFont.Tiny
                }), footer.intrinsicHeight)
            );

            actionsGroup = listFrame.AddElement(new CElement());
            actionsGroup.StackLeft(
                actionsGroup.AddElement(new CLabel
                {
                    Title = "Select:",
                    TextAlignment = TextAnchor.MiddleLeft,
                }),
                10,
                (actionsGroup.AddElement(new CButton
                {
                    Title = "none",
                    Action = (_) => tablesModel.SelectNone(),
                }), 200),
                10,
                (actionsGroup.AddElement(new CButton
                {
                    Title = "all supported",
                    Action = (_) => tablesModel.SelectAllAtLeast(TableCompatibility.Supported),
                }), 200),
                10,
                (actionsGroup.AddElement(new CButton
                {
                    Title = "all compatible",
                    Action = (_) => tablesModel.SelectAllAtLeast(TableCompatibility.Compatible),
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
                case TableCompatibility.Compatible:
                    return Color.white;
                case TableCompatibility.Incompatible:
                    return Color.red;
                case TableCompatibility.Supported:
                    return Color.green;
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
