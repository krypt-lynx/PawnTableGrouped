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

    public class PawnTableGroupedGUI
    {
        PawnTableGroupedModel model;

        CGuiRoot host = new CGuiRoot();
        CCheckbox collapseBtn;
        CPawnTable list;

        public PawnTableGroupedGUI(PawnTableGroupedModel model)
        {
            this.model = model;

            ConstructGUI();
        }
        public void SetInnerWidth(float innerWidth)
        {
            list.InnerWidth = innerWidth;
        }


        void ConstructGUI()
        {
            CElement footer;

            list = host.AddElement(new CPawnTable());
            list.allowHScroll = model.AllowHScroll;

            var weakThis = new Verse.WeakReference<PawnTableGroupedGUI>(this);

            footer = host.AddElement(new CElement());

            CRowSegment headerSegment = new CPawnListHeader(model, new RangeInt(0, 1), true);
            CRowSegment bodySegment = new CPawnListHeader(model, new RangeInt(0, int.MaxValue / 2), true); // including first column twice for Nubmers compatibility; second copy is invisible
            var header = list.AppendRow(
                new CPawnTableRow
                {
                    Fixed = headerSegment,
                    Row = bodySegment,
                });
            list.TableHeader = header;
            headerSegment.AddConstraint(headerSegment.height ^ headerSegment.intrinsicHeight);
            bodySegment.AddConstraint(bodySegment.height ^ bodySegment.intrinsicHeight);

            Texture2D settingsTex = new Resource<Texture2D>("UI/Settings");
            var GroupBtn = footer.AddElement(new CWidget
            {
                DoWidgetContent = (_, bounds) =>
                {
                    Widgets.Dropdown(bounds, weakThis?.Target, t => t,
                    t =>
                    {
                        return t.CreateGroupingMenuOptions();
                    }, null, settingsTex);
                }
            });
            var DecendingSortBtn = footer.AddElement(new CCheckbox
            {
                TextureChecked = new Resource<Texture2D>("UI/OrderDec"),
                TextureUnchecked = new Resource<Texture2D>("UI/OrderAsc"),
                Checked = model.SortDecending,
                Changed = (_, value) =>
                {
                    var this_ = weakThis.Target;
                    this_.model.SetSortingDecending(value);
                    this_.PopulateList();
                },
            });
            collapseBtn = footer.AddElement(new CCheckbox
            {
                TextureChecked = new Resource<Texture2D>("UI/Expand"),
                TextureUnchecked = new Resource<Texture2D>("UI/Collapse"),
                Changed = (sender, _) =>
                {
                    sender.Checked = weakThis.Target.model.ExpandCollapse();
                },
            });


            // arranging table elements
            host.AddConstraints(list.left ^ host.left, list.top ^ host.top, 
                list.right ^ host.right, list.bottom ^ host.bottom);

            // attaching footer bellow table
            host.AddConstraints(footer.top ^ list.bottom, footer.left ^ list.left, footer.right ^ list.right, footer.height ^ Metrics.PawnTableFooterHeight);
            //fotterBtnOffset = 50;

            // arranging buttons in footer
            footer.StackRight(StackOptions.Create(constrainEnd: false), 16, model.FotterBtnOffset, (GroupBtn, 30), (DecendingSortBtn, 30), (collapseBtn, 30));
           

            model.GroupsStateChanged = (m) =>
            {
                collapseBtn.Checked = !m.Groups.Any(x => m.IsExpanded(x));
            };
        }

        private IEnumerable<Widgets.DropdownMenuElement<PawnTableGroupedGUI>> CreateGroupingMenuOptions()
        {
            foreach (var worker in model.PredefinedGroupWorkers)
            {
                yield return new Widgets.DropdownMenuElement<PawnTableGroupedGUI>
                {
                    option = new FloatMenuOption(worker.MenuItemTitle(), () =>
                    {
                        model.SetGrouper(worker);
                    })
                };
            }

            if (Mod.Settings.groupByColumnExperimental)
            {
                yield return new Widgets.DropdownMenuElement<PawnTableGroupedGUI>
                {
                    option = new FloatSubmenuOption("by column:", () =>
                    {
                        return model.ColumnGroupWorkers.Select(x => new FloatMenuOption(x.MenuItemTitle(), () =>
                        {
                            model.SetGrouper(x);
                        })).ToList();
                    })
                };
            }
        }


        public void PopulateList()
        {
            list.ClearRows();

            var columnWidths = model.accessor.cachedColumnWidths;
            var column0Width = columnWidths.Count > 0 ? columnWidths[0] : 0;
            list.FixedSegmentWidth = column0Width + Metrics.TableLeftMargin;

            var weakThis = new Verse.WeakReference<PawnTableGroupedGUI>(this);

            foreach (var group in model.Groups)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1)
                {
                    CPawnListGroupFixed headerSegment = new CPawnListGroupFixed(group, model.IsExpanded(group));
                    CRowSegment bodySegment = Mod.Settings.disableGroupCells ? (CRowSegment)new CPawnListGroupNoSummary() : (CRowSegment)new CPawnListGroupSummary(model.Table, model.accessor, group);
                    var groupRow = list.AppendRow(
                        new CPawnListGroupRow
                        {
                            Fixed = headerSegment,
                            Row = bodySegment,
                            Group = group,
                            Action = (sectionRow) =>
                            {
                                weakThis.Target.model.SwitchExpanded(sectionRow.Group);
                            }
                        });


                    headerSegment.AddConstraint(headerSegment.height ^ headerSegment.intrinsicHeight);
                    bodySegment.AddConstraint(bodySegment.height ^ bodySegment.intrinsicHeight);
                    

                }
                
                if (model.IsExpanded(group))
                {
                    foreach (var pawn in group.Pawns)
                    {
                        CRowSegment headerSegment = new CPawnListRow(model.Table, model.accessor, group, pawn, new RangeInt(0, 1), true);
                        CPawnListRow bodySegment = new CPawnListRow(model.Table, model.accessor, group, pawn, new RangeInt(1, int.MaxValue / 2), false);
                        CPawnListHighlight background = new CPawnListHighlight(pawn);

                        var pawnRow = list.AppendRow(
                            new CPawnTableRow
                            {
                                Fixed = headerSegment,
                                Row = bodySegment,
                                Background = background
                            });


                        headerSegment.AddConstraint(headerSegment.height ^ headerSegment.intrinsicHeight);
                        bodySegment.AddConstraint(bodySegment.height ^ bodySegment.intrinsicHeight);
                    }
                }
            }
        }

        public void OnGUI(Vector2 position, int magic)
        {
            host.InRect = new Rect((int)position.x, (int)position.y, (int)model.accessor.cachedSize.x, (int)model.accessor.cachedSize.y);
            host.UpdateLayoutIfNeeded();
            host.DoElementContent();
        }

        public float CalculateTotalRequiredHeight()
        {

            float height = model.cachedHeaderHeight;
            foreach (var section in model.Groups)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1)
                {
                    height += Metrics.GroupHeaderHeight;
                }
                if (model.IsExpanded(section))
                {
                    foreach (var pawn in section.Pawns)
                    {
                        height += model.CalculateRowHeight(pawn);
                    }
                }
            }

            height += model.TableExtendedArea;

            return height;
        }
    }

}
