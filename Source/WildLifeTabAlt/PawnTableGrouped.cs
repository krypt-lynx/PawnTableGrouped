using Cassowary;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt
{
    public static class Metrics {
        public const float TableLeftMargin = 8;
    }

    public interface IPawnTableGrouped
    {
        void override_RecacheIfDirty();
        void override_PawnTableOnGUI(Vector2 position);

        float override_GetOptimalWidth(PawnColumnDef column);
        float override_GetMinWidth(PawnColumnDef column);
        float override_GetMaxWidth(PawnColumnDef column);

        float override_CalculateTotalRequiredHeight();
        
    }

    public class PawnTableGroupedImpl
    {
        PawnTableAccessor accessor;
        PawnTable table; // todo: weak ref
        List<GroupColumnWorker> columnResolvers;
        private PawnTableDef def;


        public PawnTableGroupedImpl(PawnTable table, PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight)
        {
            this.table = table;
            this.def = def;
            columnResolvers = new List<GroupColumnWorker>();
            accessor = new PawnTableAccessor(table);

            table.SetDirty();
            ConstructGUI();
        }

        CGuiRoot host = new CGuiRoot();
        CListView list;
        CElement header;

        void ConstructGUI()
        {
            header = host.AddElement(new CPawnListHeader(table, accessor));
            list = host.AddElement(new CListView
            {
                ShowScrollBar = CScrollBarMode.Show
            });

            host.StackTop((header, header.intrinsicHeight), list);
        }

        private void PopulateList(IEnumerable<PawnTableGroup> groups)
        {
            list.ClearRows();

            foreach (var group in groups)
            {
                var row = new CPawnListSection(table, accessor, group, IsExpanded(group));
                row.Action = (sectionRow) =>
                {
                    var g = ((CPawnListSection)sectionRow).Group;
                    SetExpanded(g, !IsExpanded(g));
                    table.SetDirty();
                };

                list.AppendRow(row);
                row.AddConstraint(row.height ^ 30);

                if (IsExpanded(group))
                {
                    foreach (var pawn in group.Pawns)
                    {
                        var pawnRow = list.AppendRow(new CPawnListRow(table, accessor, group, pawn));

                        pawnRow.AddConstraint(pawnRow.height ^ pawnRow.intrinsicHeight);
                    }
                }
            }
        }


        List<PawnTableGroup> sections;
        Dictionary<string, bool> ExpandedState = new Dictionary<string, bool>();
        bool IsExpanded(PawnTableGroup group)
        {
            if (ExpandedState.ContainsKey(group.Title))
            {
                return ExpandedState[group.Title];
            }
            else
            {
                return true;
            }
        }

        void SetExpanded(PawnTableGroup group, bool expanded)
        {
            ExpandedState[group.Title] = expanded;
        }

        public void RecacheGroups()
        {
            //  IEqualityComparer
            // var groups = PawnsListForReading.GroupBy(x => x, new ByColumnComparer(DefDatabase<PawnColumnDef>.GetNamed("Label")));

            // return input.OrderByDescending((Pawn p) => p.RaceProps.baseBodySize).ThenBy((Pawn p) => p.def.label);

            var groups = table.PawnsListForReading.GroupBy(p => p.kindDef.race).OrderByDescending(g => g.Key.race.baseBodySize).ThenBy(g => g.Key.label);
            sections = groups.Select(x => new PawnTableGroup(x.Key, x, columnResolvers)).ToList();
        }

        public void RecacheColumnResolvers()
        {
            columnResolvers.Clear();
            foreach (var column in table.ColumnsListForReading)
            {
                var resolverDef = GroupColumnWorker.GetResolverSilentFail(column);
                columnResolvers.Add(resolverDef?.Worker);
            }
        }

        public float CalculateTotalRequiredHeight()
        {
            float height = accessor.cachedHeaderHeight;
            foreach (var section in sections)
            {
                height += 30;
                if (IsExpanded(section))
                {
                    foreach (var pawn in section.Pawns)
                    {
                        height += accessor.CalculateRowHeight(pawn);
                    }
                }
            }

            return height;
        }

        public void PawnTableOnGUI(Vector2 position)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            accessor.RecacheIfDirty();

            host.InRect = new Rect((int)position.x, (int)position.y, (int)accessor.cachedSize.x, (int)accessor.cachedSize.y);
            host.UpdateLayoutIfNeeded();
            host.DoElementContent();
        }

        public void RecacheIfDirty()
        {
            if (!accessor.dirty)
            {
                return;
            }
            accessor.dirty = false;
            RecacheColumnResolvers();
            accessor.RecachePawns();
            RecacheGroups();

            accessor.RecacheRowHeights();
            accessor.cachedHeaderHeight = accessor.CalculateHeaderHeight();
            accessor.cachedHeightNoScrollbar = CalculateTotalRequiredHeight();
            accessor.RecacheSize();
            var oldSize = accessor.cachedSize;
            accessor.cachedSize = new Vector2(oldSize.x + Metrics.TableLeftMargin, oldSize.y); // expand table for collapse indicator
            RecacheColumnWidths();
            accessor.RecacheLookTargets();

            PopulateList(sections);
        }


        public void RecacheColumnWidths()
        {
            ClSimplexSolver solver = new ClSimplexSolver();
            solver.AutoSolve = false;

            var cachedColumnWidths = accessor.cachedColumnWidths;

            float width = accessor.cachedSize.x - 16f - Metrics.TableLeftMargin;

            List<float> columnOptimal = new List<float>();

            // variables representing column widths
            List<ClVariable> columnVars = new List<ClVariable>();

            // expression for summ of column widths
            ClLinearExpression widthExpr = new ClLinearExpression();
            HashSet<int> distinctPriorities = new HashSet<int>();

            // summ of optimal column widths
            float optimalWidth = 0;

            foreach (var column in def.columns)
            {
                float optimalColumnWidth;
                if (column.ignoreWhenCalculatingOptimalTableSize)
                {
                    optimalColumnWidth = 0;
                }
                else
                {
                    optimalColumnWidth = GetOptimalWidth(column);
                }

                optimalWidth += optimalColumnWidth;
                columnOptimal.Add(optimalColumnWidth);
                var columnVar = new ClVariable(column.defName); // name is used for debug purposes only
                columnVars.Add(columnVar);

                widthExpr.AddExpression(columnVar); // building summ

                distinctPriorities.Add(column.widthPriority);
            }

            // variable representing flexability of columns. Will be equal to 1 if all column widths are optimal and something else if not. 
            var flex = new ClVariable("flex");

            // constraining summ of column widths to window width
            solver.AddConstraint(widthExpr ^ width, ClStrength.Strong);

            // To make prorities work sorting them in order and using order as priority in solver
            // And hope for the best, because of priorities bug.
            // But we are fine if we have less then 10 different priorities (2^n < 1000)

            var priorities = distinctPriorities.ToList();
            priorities.Sort();

            var orderForPriority = priorities.Select((x, i) => (x, i)).ToDictionary(xi => xi.Item1, xi => xi.Item2);

            // to make columns with equal priorities break together bind them together with ClStrength.Strong priority 
            var priorityVars = priorities.Select(x => new ClVariable($"priority_{x}")).ToArray();

            for (int i = 0; i < def.columns.Count; i++)
            {
                var column = def.columns[i];
                var p = orderForPriority[column.widthPriority];

                var columnWidthExpr = priorityVars[p] * columnOptimal[i] * width / optimalWidth - columnVars[i]; // == 0. Equation for width proportional resize
                solver.AddConstraint(new ClLinearConstraint(columnWidthExpr, ClStrength.Strong));

                // Limit size min/max 
                solver.AddConstraint(columnVars[i] >= GetMinWidth(column), ClStrength.Medium);
                solver.AddConstraint(columnVars[i] <= GetMaxWidth(column), ClStrength.Medium);
            }


            for (int i = 0; i < priorities.Count; i++)
            {
                var priorityVar = priorityVars[i];
                // binding priority variables to flex var
                solver.AddConstraint(new ClLinearConstraint(priorityVar - flex, ClStrength.Weak, Mathf.Pow(2, i)));
            }

            // do the magic
            solver.Solve();

            // copy results to widths cache
            cachedColumnWidths.Clear();
            for (int i = 0; i < def.columns.Count; i++)
            {
                cachedColumnWidths.Add((float)columnVars[i].Value);
            }
        }


        public float GetOptimalWidth(PawnColumnDef column)
        {
            return Mathf.Max((float)column.Worker.GetOptimalWidth(table), 0f);
        }

        public float GetMinWidth(PawnColumnDef column)
        {
            return Mathf.Max((float)column.Worker.GetMinWidth(table), 0f);
        }

        public float GetMaxWidth(PawnColumnDef column)
        {
            return Mathf.Max((float)column.Worker.GetMaxWidth(table), 0f);
        }
    }


    public class PawnTable_WildlifeGrouped : PawnTable_Wildlife, IPawnTableGrouped
    {
        PawnTableGroupedImpl impl;

        public PawnTable_WildlifeGrouped(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
            impl = new PawnTableGroupedImpl(this, def, pawnsGetter, uiWidth, uiHeight);
        }

        public void override_PawnTableOnGUI(Vector2 position)
        {
            impl.PawnTableOnGUI(position);
        }

        public void override_RecacheIfDirty()
        {
            impl.RecacheIfDirty();
        }
  
        public float override_CalculateTotalRequiredHeight()
        {
            return impl.CalculateTotalRequiredHeight();
        }

        public float override_GetOptimalWidth(PawnColumnDef column)
        {
            return impl.GetOptimalWidth(column);
        }

        public float override_GetMinWidth(PawnColumnDef column)
        {
            return impl.GetMinWidth(column);
        }

        public float override_GetMaxWidth(PawnColumnDef column)
        {
            return impl.GetMaxWidth(column);
        }
    }

    public class PawnTable_AnimalsGrouped : PawnTable_Animals, IPawnTableGrouped
    {
        PawnTableGroupedImpl impl;

        public PawnTable_AnimalsGrouped(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
            impl = new PawnTableGroupedImpl(this, def, pawnsGetter, uiWidth, uiHeight);
        }

        public void override_PawnTableOnGUI(Vector2 position)
        {
            impl.PawnTableOnGUI(position);
        }

        public void override_RecacheIfDirty()
        {
            impl.RecacheIfDirty();
        }

        public float override_CalculateTotalRequiredHeight()
        {
            return impl.CalculateTotalRequiredHeight();
        }

        public float override_GetOptimalWidth(PawnColumnDef column)
        {
            return impl.GetOptimalWidth(column);
        }

        public float override_GetMinWidth(PawnColumnDef column)
        {
            return impl.GetMinWidth(column);
        }

        public float override_GetMaxWidth(PawnColumnDef column)
        {
            return impl.GetMaxWidth(column);
        }
    }

}
