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
        public const float GroupHeaderHeight = 30;
    }

    public interface IPawnTableGrouped
    {
        void override_RecacheIfDirty();
        void override_PawnTableOnGUI(Vector2 position);

        float override_CalculateTotalRequiredHeight();
        
    }

    public class PawnTableGroupedImpl
    {
        PawnTableAccessor accessor;
        PawnTable table; // todo: weak ref
        List<GroupColumnWorker> columnResolvers;

        public PawnTableGroupedImpl(PawnTable table)
        {
            this.table = table;
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
                var groupRow = new CPawnListSection(table, accessor, group, IsExpanded(group));
                groupRow.Action = (sectionRow) =>
                {
                    var g = ((CPawnListSection)sectionRow).Group;
                    SetExpanded(g, !IsExpanded(g));
                    table.SetDirty();
                };

                list.AppendRow(groupRow);
                groupRow.AddConstraint(groupRow.height ^ groupRow.intrinsicHeight);

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
                height += Metrics.GroupHeaderHeight;
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
            accessor.RecacheColumnWidths();
            accessor.RecacheLookTargets();

            PopulateList(sections);
        }
    }

    /*
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
    }
    */
}
