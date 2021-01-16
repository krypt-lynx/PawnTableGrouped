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
    class ColumnsSettingsTab : CTabPage, IListViewDataSource
    {
        List<PawnColumnDef> columnDefs;
        float rowHeight;
        CListingRow firstRow;
        public ColumnsSettingsTab()
        {
            ReadColumnDefs();

            if (columnDefs.Count > 0) // this is a hack to resolve row height using constraints
            {
                firstRow = ListingRowForRowAt(0);
                firstRow.InRect = new Rect(0, 0, 100, 0);
                firstRow.UpdateLayoutIfNeeded();

                rowHeight = firstRow.Bounds.height;
            }
        }

        private void ReadColumnDefs()
        {
            columnDefs = DefDatabase<GroupColumnWorkerDef>.AllDefs.Select(x => x.Worker.ColumnDef).Concat(
          DefDatabase<PawnColumnDef>.AllDefs.Where(x => GroupColumnDefResolver.GetResolver(x, false) == null)
            ).ToList();
        }

        public override string Title()
        {
            return "PTG_Columns".Translate();
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

            Color getColor(GroupColumnWorkerDef def)
            {
                if (def == null)
                    return new Color(1, 1, 1, 0.4f);
                if (def.workerClass == null) // something wend wrong, should never happen
                    return Color.red;
                if (def.workerClass == typeof(GroupColumnWorker_Dummy))
                    return Color.yellow;
                return Color.white;
            } 

            var column2 = row.AddElement(new CLabel
            {
                Color = getColor(resolver),
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
        CListView_vNext columnsList = null;
        private void ConstructColumnsTab()
        {
            Verse.WeakReference<ColumnsSettingsTab> weakThis = new Verse.WeakReference<ColumnsSettingsTab>(this);

            CElement tabFrame = AddElement(new CFrame());
            this.Embed(tabFrame);

            var buttonsPanel = AddElement(new CElement());

            var resolveTable = buttonsPanel.AddElement(new CButtonText
            {
                Title = "ResolveAllColumns".Translate(),
                Action = (_) =>
                {
                    if (weakThis?.Target is var this_ && this_ != null)
                    {
                        foreach (var def in this_.columnDefs)
                        {
                            GroupColumnDefResolver.GetResolver(def);
                        }
                        this_.ReadColumnDefs();
                        this_.firstRow = null;
                        this_.columnsList.UpdateList();
                    }
                }
            });
            var logColumns = buttonsPanel.AddElement(new CButtonText
            {
                Title = "LogColumns".Translate(),
                Action = (_) =>
                {
                    if (weakThis?.Target is var this_ && this_ != null)
                    {
                        string msg = string.Join("\n",
                            DefDatabase<PawnColumnDef>.AllDefs
                                .Select(x => (columnDef: x, resolver: GroupColumnDefResolver.GetResolver(x, false)))
                                .Select(x => $"column: defName={x.columnDef.defName}; " +
                                $"worker: {x.columnDef.workerClass.FullName}; " + 
                                $"group worker: {(x.resolver != null).Case(() => x.resolver.workerClass?.FullName, () => "<unresolved>")}")
                            );
                        Log.Message(msg);
                    }
                }
            });

            buttonsPanel.StackRight(StackOptions.Create(constrainEnd:false),
                (logColumns, 200), 
                10,
                (resolveTable, 200));
            resolveTable.AddConstraint(resolveTable.height ^ 30);

            columnsList = tabFrame.AddElement(new CListView_vNext());
            tabFrame.StackTop(StackOptions.Create(insets: new EdgeInsets(3)),
                buttonsPanel,
                2,
                columnsList);
            columnsList.DataSource = this;
        }
    }

}
