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
        CElement header;

        public PawnTableGroupedGUI(PawnTableGroupedModel model)
        {
            this.model = model;

            ConstructGUI();
        }
        public void SetInnerWidth(float innerWidth)
        {
            list.InnerWidth = innerWidth;
        }

        float extendedArea = 30;
        float fotterBtnOffset = 0;

        void ConstructGUI()
        {
            CElement footer;

            list = host.AddElement(new CPawnTable());
            var weakList = new Verse.WeakReference<CPawnTable>(list);
            header = host.AddElement(new PawnListHeader(model, () => weakList.Target.OuterScrollPosition.x));
            footer = host.AddElement(new CElement());

            Texture2D img1 = new Resource<Texture2D>("UI/Settings");
            var GroupBtn = footer.AddElement(new CWidget
            {
                DoWidgetContent = (_, bounds) =>
                {
                    Widgets.Dropdown(bounds, model.ActiveGrouper, g => g,
                    t =>
                    {
                        return model.AllGroupers.Select(g =>
                            new Widgets.DropdownMenuElement<GroupWorker>
                            {
                                option = new FloatMenuOption(g.MenuItemTitle(), () =>
                                {
                                    model.SetGrouper(g);
                                })
                            });
                    }, null, img1);
                }
            });
            var DecendingSortBtn = footer.AddElement(new CCheckbox
            {
                TextureChecked = new Resource<Texture2D>("UI/OrderDec"),
                TextureUnchecked = new Resource<Texture2D>("UI/OrderAsc"),
                Checked = model.SortDecending,
                Changed = (_, value) =>
                {
                    model.SetSortingDecending(value);
                    PopulateList();
                },
            });
            collapseBtn = footer.AddElement(new CCheckbox
            {
                TextureChecked = new Resource<Texture2D>("UI/Expand"),
                TextureUnchecked = new Resource<Texture2D>("UI/Collapse"),
                Changed = (sender, _) =>
                {
                    sender.Checked = model.ExpandCollapse();
                },
            });


            // arranging table elements
            host.AddConstraints(header.left ^ host.left, header.top ^ host.top,
                header.right ^ host.right, header.height ^ header.intrinsicHeight);
            host.AddConstraints(list.left ^ host.left, list.top ^ header.bottom, 
                list.right ^ host.right, list.bottom ^ host.bottom);


            // attaching footer bellow table
            host.AddConstraints(footer.top ^ list.bottom, footer.left ^ list.left, footer.right ^ list.right, footer.height ^ Metrics.PawnTableFooterHeight);
            //fotterBtnOffset = 50;

            // arranging buttons in footer
            footer.StackRight(StackOptions.Create(constrainEnd: false), 16, fotterBtnOffset, (GroupBtn, 30), (DecendingSortBtn, 30), (collapseBtn, 30));
           

            model.GroupsStateChanged = (m) =>
            {
                collapseBtn.Checked = !model.Groups.Any(x => model.IsExpanded(x));
            };
        }

        public void PopulateList()
        {
            list.ClearRows();

            var columnWidths = model.accessor.cachedColumnWidths;
            var column0Width = columnWidths.Count > 0 ? columnWidths[0] : 0;
            list.FixedSegmentWidth = column0Width + Metrics.TableLeftMargin;

            foreach (var group in model.Groups)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1)
                {
                    CRowSegment headerSegment = null;
                    CPawnListGroup bodySegment = new CPawnListGroup(model.Table, model.accessor, group, model.IsExpanded(group));
                    var groupRow = list.AppendRow(
                        new CPawnTableRow2
                        {
                            Pinned = headerSegment,
                            Row = bodySegment
                        });


                    bodySegment.Action = (sectionRow) =>
                    {
                        var g = ((CPawnListGroup)sectionRow).Group;
                        model.SwitchExpanded(g);
                    };
                    bodySegment.AddConstraint(bodySegment.height ^ bodySegment.intrinsicHeight);
                    

                }
                
                if (model.IsExpanded(group))
                {
                    foreach (var pawn in group.Pawns)
                    {
                        CRowSegment headerSegment = new CPawnListRow(model.Table, model.accessor, group, pawn, new RangeInt(0, 1), true);
                        CPawnListRow bodySegment = new CPawnListRow(model.Table, model.accessor, group, pawn, new RangeInt(1, columnWidths.Count - 1), false);
                        var pawnRow = list.AppendRow(
                            new CPawnTableRow2
                            {
                                Pinned = headerSegment,
                                Row = bodySegment
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
            height += Metrics.PawnTableFooterHeight;

            height -= extendedArea;

            return height;
        }
    }

}
