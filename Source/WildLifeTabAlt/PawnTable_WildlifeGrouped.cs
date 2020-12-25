using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt
{
    public interface IPawnTableGrouped
    {
        bool override_RecacheIfDirty_Prefix(out bool wasDirty);
        void override_RecacheIfDirty_Postfix(bool wasDirty);
        bool override_PawnTableOnGUI_Prefix(Vector2 position);
        void override_PawnTableOnGUI_Postfix(Vector2 position);
    }

    public class PawnTableAccessor // Accessors
    {
        private Verse.WeakReference<PawnTable> table;
        private PawnTable Table => table?.Target;

        public PawnTableAccessor(PawnTable table)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
        }

        public bool dirty
        {
            get
            {
                return (bool)typeof(PawnTable).GetField("dirty", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("dirty", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public float cachedHeaderHeight
        {
            get
            {
                return (float)typeof(PawnTable).GetField("cachedHeaderHeight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedHeaderHeight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public float cachedHeightNoScrollbar
        {
            get
            {
                return (float)typeof(PawnTable).GetField("cachedHeightNoScrollbar", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedHeightNoScrollbar", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public List<Pawn> cachedPawns
        {
            get
            {
                return (List<Pawn>)typeof(PawnTable).GetField("cachedPawns", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedPawns", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public Vector2 cachedSize
        {
            get
            {
                return (Vector2)typeof(PawnTable).GetField("cachedSize", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedSize", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public List<float> cachedColumnWidths
        {
            get
            {
                return (List<float>)typeof(PawnTable).GetField("cachedColumnWidths", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedColumnWidths", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }



        public void RecachePawns()
        {
            typeof(PawnTable).GetMethod("RecachePawns", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheRowHeights()
        {
            typeof(PawnTable).GetMethod("RecacheRowHeights", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheSize()
        {
            typeof(PawnTable).GetMethod("RecacheSize", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheColumnWidths()
        {
            typeof(PawnTable).GetMethod("RecachePawns", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheLookTargets()
        {
            typeof(PawnTable).GetMethod("RecacheLookTargets", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public float CalculateHeaderHeight()
        {
            return (float)typeof(PawnTable).GetMethod("CalculateHeaderHeight", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public float CalculateTotalRequiredHeight()
        {
            return (float)typeof(PawnTable).GetMethod("CalculateTotalRequiredHeight", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public bool CanAssignPawn(Pawn p)
        {
            return (bool)typeof(PawnTable).GetMethod("CanAssignPawn", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { p });
        }
        public float CalculateRowHeight(Pawn p)
        {
            return (float)typeof(PawnTable).GetMethod("CalculateRowHeight", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { p });
        }
        public void RecacheIfDirty()
        {
            typeof(PawnTable).GetMethod("RecacheIfDirty", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });            
        }
    }

    public class PawnTable_WildlifeGrouped : PawnTable_Wildlife, IPawnTableGrouped
    {
        class ByColumnComparer : IEqualityComparer<Pawn>
        {
            private PawnColumnDef groupBy;

            public ByColumnComparer(PawnColumnDef groupBy)
            {
                this.groupBy = groupBy;
            }

            public bool Equals(Pawn x, Pawn y)
            {
                return groupBy.Worker.Compare(x, y) == 0;
            }

            public int GetHashCode(Pawn obj)
            {
                return 0; // have no cache; TODO: test performance
            }
        }

        PawnTableAccessor accessor;

        public PawnTable_WildlifeGrouped(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
            accessor = new PawnTableAccessor(this);

            this.SetDirty();
            ConstructGUI();
        }

        public bool override_PawnTableOnGUI_Prefix(Vector2 position)
        {
            if (Event.current.type == EventType.Layout)
            {
                return false;
            }
            accessor.RecacheIfDirty();

            host.InRect = new Rect((int)position.x, (int)position.y, (int)accessor.cachedSize.x, (int)accessor.cachedSize.y);
            host.UpdateLayoutIfNeeded();
            host.DoElementContent();

            return false;
        }

        public void override_PawnTableOnGUI_Postfix(Vector2 position) { }


        public bool override_RecacheIfDirty_Prefix(out bool wasDirty)
        {
            wasDirty = accessor.dirty;
            return true;
        }

        public void override_RecacheIfDirty_Postfix(bool wasDirty)
        {
            if (wasDirty)
            {
                RecacheGroups();
            }
        }

        private void RecacheGroups()
        {
            //  IEqualityComparer
            var groups = PawnsListForReading.GroupBy(x => x, new ByColumnComparer(DefDatabase<PawnColumnDef>.GetNamed("Label")));

            PopulateList(groups);

            Log.Message(groups.ToString());
        }


        private void PopulateList(IEnumerable<IGrouping<Pawn, Pawn>> groups)
        {
            list.ClearRows();

            float width = accessor.cachedSize.x - 16f;

            foreach (var group in groups)
            {
                var row = list.AppendRow(new CListingRow());

                row.Embed(row.AddElement(new CLabel
                {
                    Title = group.Key.ToString(),
                    Font = GameFont.Small,
                    TextAlignment = TextAnchor.MiddleLeft,
                }));
                row.AddConstraint(row.height ^ 30);


                foreach (var pawn in group)
                {
                    var rowHeight = accessor.CalculateRowHeight(pawn);

                    row = list.AppendRow(new CListingRow());
                    row.Embed(row.AddElement(new CWidget
                    {
                        DoWidgetContent = (_, bounds) => {
                            int x = (int)bounds.xMin;

                            GUI.color = new Color(1f, 1f, 1f, 0.2f);
                            Widgets.DrawLineHorizontal((int)bounds.xMin, bounds.yMin, bounds.width);
                            GUI.color = Color.white;
                            if (!this.CanAssignPawn(pawn))
                            {
                                GUI.color = Color.gray;
                            }

                            if (Mouse.IsOver(bounds))
                            {
                                GUI.DrawTexture(bounds, TexUI.HighlightTex);
                                //this.cachedLookTargets[rowIndex].Highlight(true, this.cachedPawns[rowIndex].IsColonist, false);
                            }
                            for (int columnIndex = 0; columnIndex < ColumnsListForReading.Count; columnIndex++)
                            {
                                int columnWidth;
                                if (columnIndex == ColumnsListForReading.Count - 1)
                                {
                                    columnWidth = (int)(width - x);
                                }
                                else
                                {
                                    columnWidth = (int)accessor.cachedColumnWidths[columnIndex];
                                }
                                Rect cellRect = new Rect(x, bounds.yMin, columnWidth, (int)rowHeight);
                                ColumnsListForReading[columnIndex].Worker.DoCell(cellRect, pawn, this);
                                x += columnWidth;
                            }
                            if (pawn.Downed)
                            {
                                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                                Widgets.DrawLineHorizontal(bounds.xMin, bounds.center.y, bounds.width);
                            }
                            GUI.color = Color.white;
                        }

                    }));
                    row.AddConstraint(row.height ^ rowHeight);

                }

            }
        }

        CGuiRoot host = new CGuiRoot();
        CListView list;
        CWidget header;


        void ConstructGUI()
        {
            header = host.AddElement(new CWidget
            {
                TryFitContent = (_) => new Vector2(0, (int)accessor.cachedHeaderHeight),
                DoWidgetContent = (_, bounds) =>
                {
                    float width = bounds.width - 16f;
                    int x = 0;
                    for (int headerColumnIndex = 0; headerColumnIndex < ColumnsListForReading.Count; headerColumnIndex++)
                    {
                        int columnWidth;
                        if (headerColumnIndex == this.ColumnsListForReading.Count - 1)
                        {
                            columnWidth = (int)(width - x);
                        }
                        else
                        {
                            columnWidth = (int)accessor.cachedColumnWidths[headerColumnIndex];
                        }
                        Rect rect = new Rect(((int)bounds.xMin + x), (int)bounds.yMin, columnWidth, bounds.height);
                        ColumnsListForReading[headerColumnIndex].Worker.DoHeader(rect, this);
                        x += columnWidth;
                    }
                }
            });
            list = host.AddElement(new CListView());


            host.StackTop((header, header.intrinsicHeight), list);
        }


    }

    /*

    public partial class PawnTableGrouped
    {

        class ByColumnComparer : IEqualityComparer<Pawn>
        {
            private PawnColumnDef groupBy;

            public ByColumnComparer(PawnColumnDef groupBy)
            {
                this.groupBy = groupBy;
            }

            public bool Equals(Pawn x, Pawn y)
            {
                return groupBy.Worker.Compare(x, y) == 0;
            }

            public int GetHashCode(Pawn obj)
            {
                return 0; // have no cache; TODO: test performance
            }
        }

        private PawnTableGroupDef groupDef;
        public PawnTableGrouped(PawnTable table, PawnTableGroupDef groupDef)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
            this.groupDef = groupDef;

            PawnTablePatches.RegisterPawnTableExtension(table, this);

            ConstructGUI();
            Table.SetDirty();
        }


        public void override_RecacheIfDirty()
        {
            if (!this.dirty)
            {
                return;
            }

            this.dirty = false;
            this.RecachePawns();
            //this.RecacheRowHeights();
            this.cachedHeaderHeight = this.CalculateHeaderHeight();
            this.cachedHeightNoScrollbar = this.CalculateTotalRequiredHeight();
            RecacheGroups();
            this.RecacheSize();
            this.RecacheColumnWidths();
            this.RecacheLookTargets();


        }

        private void RecacheGroups()
        {
            //  IEqualityComparer
            var groups = Table.PawnsListForReading.GroupBy(x => x, new ByColumnComparer(groupDef.groupBy));

            PopulateList(groups);

            Log.Message(groups.ToString());
        }

        private void PopulateList(IEnumerable<IGrouping<Pawn, Pawn>> groups)
        {
            list.ClearRows();

            float width = this.cachedSize.x - 16f;

            foreach (var group in groups)
            {
                var row = list.AppendRow(new CListingRow());

                row.Embed(row.AddElement(new CLabel
                {
                    Title = group.Key.ToString(),
                    Font = GameFont.Small,
                    TextAlignment = TextAnchor.MiddleLeft,
                }));
                row.AddConstraint(row.height ^ 30);


                foreach (var pawn in group)
                {
                    var rowHeight = CalculateRowHeight(pawn);

                    row = list.AppendRow(new CListingRow());
                    row.Embed(row.AddElement(new CWidget
                    {
                        DoWidgetContent = (_, bounds) => {
                            int x = (int)bounds.xMin;

                                GUI.color = new Color(1f, 1f, 1f, 0.2f);
                                Widgets.DrawLineHorizontal((int)bounds.xMin, bounds.yMin, bounds.width);
                                GUI.color = Color.white;
                                if (!this.CanAssignPawn(pawn))
                                {
                                    GUI.color = Color.gray;
                                }
                               
                                if (Mouse.IsOver(bounds))
                                {
                                    GUI.DrawTexture(bounds, TexUI.HighlightTex);
                                    //this.cachedLookTargets[rowIndex].Highlight(true, this.cachedPawns[rowIndex].IsColonist, false);
                                }
                                for (int columnIndex = 0; columnIndex < Table.ColumnsListForReading.Count; columnIndex++)
                                {
                                    int columnWidth;
                                    if (columnIndex == Table.ColumnsListForReading.Count - 1)
                                    {
                                        columnWidth = (int)(width - x);
                                    }
                                    else
                                    {
                                        columnWidth = (int)cachedColumnWidths[columnIndex];
                                    }
                                    Rect cellRect = new Rect(x, bounds.yMin, columnWidth, (int)rowHeight);
                                    Table.ColumnsListForReading[columnIndex].Worker.DoCell(cellRect, pawn, Table);
                                    x += columnWidth;
                                }
                                if (pawn.Downed)
                                {
                                    GUI.color = new Color(1f, 0f, 0f, 0.5f);
                                    Widgets.DrawLineHorizontal(bounds.xMin, bounds.center.y, bounds.width);
                                }
                                GUI.color = Color.white;
                            }
                        
                    }));
                    row.AddConstraint(row.height ^ rowHeight);

                }

            } 
        }

        CGuiRoot host = new CGuiRoot();
        CListView list;
        CWidget header;


        void ConstructGUI()
        {
            header = host.AddElement(new CWidget
            {
                TryFitContent = (_) => new Vector2(0, (int)cachedHeaderHeight),
                DoWidgetContent = (_, bounds) =>
                {
                    float width = bounds.width - 16f;
                    int x = 0;
                    for (int headerColumnIndex = 0; headerColumnIndex < Table.ColumnsListForReading.Count; headerColumnIndex++)
                    {
                        int columnWidth;
                        if (headerColumnIndex == Table.ColumnsListForReading.Count - 1)
                        {
                            columnWidth = (int)(width - x);
                        }
                        else
                        {
                            columnWidth = (int)cachedColumnWidths[headerColumnIndex];
                        }
                        Rect rect = new Rect(((int)bounds.xMin + x), (int)bounds.yMin, columnWidth, bounds.height);
                        Table.ColumnsListForReading[headerColumnIndex].Worker.DoHeader(rect, Table);
                        x += columnWidth;
                    }
                }
            });
            list = host.AddElement(new CListView());


            host.StackTop((header, header.intrinsicHeight), list);
        }

        public void override_PawnTableOnGUI(Vector2 position)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            this.override_RecacheIfDirty();

            host.InRect = new Rect((int)position.x, (int)position.y, (int)this.cachedSize.x, (int)this.cachedSize.y);
            host.UpdateLayoutIfNeeded();
            host.DoElementContent();

        }
    }

    */
}
