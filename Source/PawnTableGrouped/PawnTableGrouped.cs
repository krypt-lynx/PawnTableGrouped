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

        public const float GroupHeaderOpacityIcon = 0.4f;
        public const float GroupHeaderOpacityText = 0.6f;
        public readonly static Color GroupHeaderOpacityIconColor = new Color(1, 1, 1, GroupHeaderOpacityIcon);
        public readonly static Color GroupHeaderOpacityColor = new Color(1, 1, 1, GroupHeaderOpacityText);

        public const float PawnTableFooterHeight = 30;
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

            miscGroupers.Add(new GroupWorker_AllInOne());
            miscGroupers.Add(new GroupWorker_ByRace());
            miscGroupers.Add(new GroupWorker_ByGender());
            activeGrouper = miscGroupers.First();

            table.SetDirty();
            ConstructGUI();
        }

        CGuiRoot host = new CGuiRoot();
        CCheckbox collapseBtn;
        CListView list;
        CElement header;

        float extendedArea = 30;
        float fotterBtnOffset = 0;

        void ConstructGUI()
        {
            CElement footer;

            header = host.AddElement(new CPawnListHeader(table, accessor, magic));
            list = host.AddElement(new CListView
            {
                ShowScrollBar = CScrollBarMode.Show
            });
            footer = host.AddElement(new CElement());

            Texture2D img1 = new Resource<Texture2D>("UI/Settings_Ask_Wiris_Permition_To_Use");
            var GroupBtn = footer.AddElement(new CWidget
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
                    }, null, img1);
                }
            });
            var DecendingSortBtn = footer.AddElement(new CCheckbox
            {
                TextureChecked = new Resource<Texture2D>("UI/OrderDec"),
                TextureUnchecked = new Resource<Texture2D>("UI/OrderAsc"),
                Checked = sortDecending,
                Changed = (_, value) =>
                {
                    sortDecending = value;
                    SortGroups();
                    PopulateList();
                },
            });
            collapseBtn = footer.AddElement(new CCheckbox
            {
                TextureChecked = new Resource<Texture2D>("UI/Expand"),
                TextureUnchecked = new Resource<Texture2D>("UI/Collapse"),
                Changed = (sender, _) =>
                {
                    bool needToExpand = true;
                    foreach (var group in groups)
                    {
                        if (IsExpanded(group))
                        {
                            needToExpand = false;
                        }
                    }
                    foreach (var group in groups)
                    {
                        SetExpanded(group, needToExpand, false);
                    }
                    sender.Checked = !needToExpand;

                    table.SetDirty();
                },
            });

            host.StackTop((header, header.intrinsicHeight), list);
            host.AddConstraints(footer.top ^ list.bottom, footer.left ^ list.left, footer.right ^ list.right, footer.height ^ Metrics.PawnTableFooterHeight);
            //fotterBtnOffset = 50;
            footer.StackRight(StackOptions.Create(constrainEnd:false), 16, fotterBtnOffset, (GroupBtn, 30), (DecendingSortBtn, 30), (collapseBtn, 30));
        }

        private void PopulateList()
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


        List<PawnTableGroup> groups;
        bool sortDecending = false;

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

        void SetExpanded(PawnTableGroup group, bool expanded, bool updateBtnState = true)
        {
            if (group?.Title == null)
            {
                $"trying to set expanded flag for group with 'null' title".Log(LogHelper.MessageType.Warning);
                return;
            }

            ExpandedState[group.Title] = expanded;

            if (updateBtnState)
            {
                UpdateCollapseBtnState();
            }
        }

        private void UpdateCollapseBtnState()
        {
            collapseBtn.Checked = !groups.Any(x => IsExpanded(x));
        }

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
            var pawnGroups = table.PawnsListForReading
                .GroupBy(p => p, activeGrouper.GroupingEqualityComparer);

            groups = new List<PawnTableGroup>();
            foreach (var group in pawnGroups)
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
                groups.Add(new PawnTableGroup(activeGrouper.TitleForGroup(group, group.Key), group.Key, pawns, columnResolvers));
            }

            SortGroups();


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

        private void SortGroups()
        {
            if (sortDecending)
            {
                groups.Sort((a, b) => activeGrouper.GroupsSortingComparer.Compare(b, a));
            }
            else
            {
                groups.Sort(activeGrouper.GroupsSortingComparer);
            }
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
            foreach (var section in groups)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || groups.Count > 1)
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
            height += Metrics.PawnTableFooterHeight;

            return height - extendedArea;
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

            PopulateList();
            UpdateCollapseBtnState();
        }
    }

}
