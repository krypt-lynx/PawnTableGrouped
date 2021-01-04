using Cassowary;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public static class Metrics {
        public const float TableLeftMargin = 8;
        public const float GroupHeaderHeight = 30;
        public const float GroupTitleRightMargin = 8;
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
        PawnTableDef def;
        List<GroupColumnWorker> columnResolvers;

        public PawnTableGroupedImpl(PawnTable table, PawnTableDef def)
        {
            this.table = table;
            this.def = def;
            columnResolvers = new List<GroupColumnWorker>();
            accessor = new PawnTableAccessor(table);
            miscGroupers = new List<GroupWorker>();

            miscGroupers.Add(new GroupWorker_ByRace());
            activeGrouper = miscGroupers.First();

            table.SetDirty();
            ConstructGUI();
        }

        CGuiRoot host = new CGuiRoot();
        CListView list;
        CElement header;

        

        void ConstructGUI()
        {
            header = host.AddElement(new CPawnListHeader(table, accessor, magic));
            list = host.AddElement(new CListView
            {
                ShowScrollBar = CScrollBarMode.Show
            });
            var btn = host.AddElement(new CWidget
            {
                //Title = "#",
                //Action = (sender) =>
                DoWidgetContent = (_, bounds) =>
                {
                    Widgets.Dropdown(bounds, activeGrouper, g => g,
                    t =>
                    {
                        return AllGroupers.Select(g =>
                            new Widgets.DropdownMenuElement<GroupWorker>
                            {
                                option = new FloatMenuOption(g.MenuItemTitle(), () =>
                                {
                                    activeGrouper = g;
                                    table.SetDirty();
                                })
                            });
                    });
                }
            });


            host.AddConstraints(header.left ^ host.left, header.top ^ host.top, header.right ^ btn.left, header.height ^ header.intrinsicHeight,
                btn.top >= host.top, btn.right ^ host.right, btn.bottom ^ header.bottom, btn.width ^ 16,
                list.left ^ host.left, list.top ^ header.bottom, list.right ^ host.right, list.bottom ^ host.bottom);
            host.AddConstraint(btn.height ^ 30, ClStrength.Weak);

            //host.StackTop((header, header.intrinsicHeight), list);
        }

        private void PopulateList(IReadOnlyCollection<PawnTableGroup> groups)
        {
            list.ClearRows();

            foreach (var group in groups)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || groups.Count > 1)
                {
                    var groupRow = (CPawnListGroup)list.AppendRow(new CPawnListGroup(table, accessor, group, IsExpanded(group)));
                    groupRow.Action = (sectionRow) =>
                    {
                        var g = ((CPawnListGroup)sectionRow).Group;
                        SetExpanded(g, !IsExpanded(g));
                        table.SetDirty();
                    };
                    groupRow.AddConstraint(groupRow.height ^ groupRow.intrinsicHeight);

                }
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
            if (group?.Title == null)
            {
                $"trying to get expanded flag for group with 'null' title".Log(LogHelper.MessageType.Warning);
                return true;
            }

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
            if (group?.Title == null)
            {
                $"trying to set expanded flag for group with 'null' title".Log(LogHelper.MessageType.Warning);
                return;
            }

            ExpandedState[group.Title] = expanded;
        }

        /*
        class ByColumnComparer : IComparer<Pawn>
        {
            private PawnColumnDef pawnColumnDef;

            public ByColumnComparer(PawnColumnDef pawnColumnDef)
            {
                this.pawnColumnDef = pawnColumnDef;
            }

            public int Compare(Pawn x, Pawn y)
            {
                return pawnColumnDef.Worker.Compare(x, y);
            }
        }
        */

        List<GroupWorker> miscGroupers;
        GroupWorker activeGrouper;

        IEnumerable<GroupWorker> AllGroupers
        {
            get
            {
                return miscGroupers.Concat(columnResolvers.Select(x => x.GroupWorker).Where(x => x != null));
            }
        }

        public void RecacheGroupers()
        {

        }

        public void RecacheGroups()
        {
            var groups = table.PawnsListForReading
                .GroupBy(p => p, activeGrouper.GroupingEqualityComparer);

            sections = new List<PawnTableGroup>();
            foreach (var group in groups)
            {
                var pawns = accessor.LabelSortFunction(group).ToList();
                if (accessor.sortByColumn != null)
                {
                    if (accessor.sortDescending)
                    {
                        pawns.SortStable(new Func<Pawn, Pawn, int>(accessor.sortByColumn.Worker.Compare));
                    }
                    else
                    {
                        pawns.SortStable((Pawn a, Pawn b) => accessor.sortByColumn.Worker.Compare(b, a));
                    }
                }
                //var pawns2 = accessor.PrimarySortFunction(pawns);
                sections.Add(new PawnTableGroup(activeGrouper.TitleForGroup(group, group.Key), group.Key, pawns, columnResolvers));
            }

            sections.Sort(activeGrouper.GroupsSortingComparer);
            
            

            /*
            var groups = table.PawnsListForReading
                .GroupBy(p => p, new ByRaceComparer())
                .OrderByDescending(g => g.Key.kindDef.race.race.baseBodySize)
                .ThenBy(g => g.Key.kindDef.race.label)
                ;
            */
            //var test = table.PawnsListForReading.OrderBy(p => p, new ByColumnComparer(def.columns[8]));

            /*
            var groups = table.PawnsListForReading
                .GroupBy(p => p.kindDef.race)
                .OrderByDescending(g => g.Key.race.baseBodySize)
                .ThenBy(g => g.Key.label);
            */

            /*

            sections = groups
                .Select(x => new PawnTableGroup(x.Key.kindDef.race.label.CapitalizeFirst() ?? "<unknown race>", x, columnResolvers)).ToList();
            */
        }

        public void RecacheColumnResolvers()
        {
            columnResolvers.Clear();
            foreach (var column in table.ColumnsListForReading)
            {
                var resolverDef = GroupColumnDefResolver.GetResolverSilentFail(column);
                columnResolvers.Add(resolverDef?.Worker);


            }
        }

        public float CalculateTotalRequiredHeight()
        {
            float height = accessor.cachedHeaderHeight;
            foreach (var section in sections)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || sections.Count > 1)
                {
                    height += Metrics.GroupHeaderHeight;
                }
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

        int magic = 0;

        public void PawnTableOnGUI(Vector2 position)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            magic = NumbersWrapper.ReorderableGroup(table);
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
            RecacheGroupers();
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

}
