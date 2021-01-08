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
        CListView list;
        CElement header;

        public PawnTableGroupedGUI(PawnTableGroupedModel model)
        {
            this.model = model;

            ConstructGUI();
        }

        float extendedArea = 30;
        float fotterBtnOffset = 0;

        void ConstructGUI()
        {
            CElement footer;

            header = host.AddElement(new CPawnListHeader(model));
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

            host.StackTop((header, header.intrinsicHeight), list);
            host.AddConstraints(footer.top ^ list.bottom, footer.left ^ list.left, footer.right ^ list.right, footer.height ^ Metrics.PawnTableFooterHeight);
            //fotterBtnOffset = 50;
            footer.StackRight(StackOptions.Create(constrainEnd: false), 16, fotterBtnOffset, (GroupBtn, 30), (DecendingSortBtn, 30), (collapseBtn, 30));

            model.GroupsStateChanged = (m) =>
            {
                collapseBtn.Checked = !model.Groups.Any(x => model.IsExpanded(x));
            };
        }

        public void PopulateList()
        {
            list.ClearRows();

            foreach (var group in model.Groups)
            {
                if (!Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1)
                {
                    var groupRow = (CPawnListGroup)list.AppendRow(new CPawnListGroup(model.Table, model.accessor, group, model.IsExpanded(group)));
                    groupRow.Action = (sectionRow) =>
                    {
                        var g = ((CPawnListGroup)sectionRow).Group;
                        model.SwitchExpanded(g);
                    };
                    groupRow.AddConstraint(groupRow.height ^ groupRow.intrinsicHeight);

                }
                if (model.IsExpanded(group))
                {
                    foreach (var pawn in group.Pawns)
                    {
                        var pawnRow = list.AppendRow(new CPawnListRow(model.Table, model.accessor, group, pawn));

                        pawnRow.AddConstraint(pawnRow.height ^ pawnRow.intrinsicHeight);
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

            return height - extendedArea;
        }
    }

}
