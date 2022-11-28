using PawnTableGrouped.TableGrid;
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
    public class PawnTableGroupedView : ICTableGridDataSource
    {
        PawnTableGroupedModel model;

        CGuiRoot host = new CGuiRoot();
        CCheckbox collapseBtn;
        CTableGrid list;

        public PawnTableGroupedView(PawnTableGroupedModel model)
        {
            this.model = model;

            ConstructGUI();
        }
        //public void SetInnerWidth(float innerWidth)
        //{
        //    list.InnerWidth = innerWidth;
        //}

       // public Action GroupsStateChanged;

        public void Invalidate()
        {
            list.Invalidate();
        }

        void ConstructGUI()
        {
            CElement footer;

            list = host.AddElement(new CTableGrid());
            list.allowHScroll = model.AllowHScroll;
            list.DataSource = this; 

            var weakThis = new Verse.WeakReference<PawnTableGroupedView>(this);

            footer = host.AddElement(new CElement());
                     
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
                    this_.Invalidate();
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
                list.SetNeedsUpdateLayout();
            };
        }

        private IEnumerable<Widgets.DropdownMenuElement<PawnTableGroupedView>> CreateGroupingMenuOptions()
        {
            foreach (var worker in model.PredefinedGroupWorkers)
            {
                yield return new Widgets.DropdownMenuElement<PawnTableGroupedView>
                {
                    option = new FloatMenuOption(worker.MenuItemTitle(), () =>
                    {
                        model.SetGrouper(worker);
                    })
                };
            }

            if (Mod.Settings.groupByColumnExperimental)
            {
                yield return new Widgets.DropdownMenuElement<PawnTableGroupedView>
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

        public int numberOfColumns()
        {
            return model.Table.Columns.Count;
        }

        public float widthForColumn(int column)
        {
            return model.Table.cachedColumnWidths[column] + (column == 0 ? Metrics.TableLeftMargin : 0);
        }

        public int numberOfSections()
        {
            return model.Groups.Count + 1;
        }

        public int numberOfRowsInSection(int section)
        {
            if (section == 0)
            {
                return 1;
            } 
            else
            {
                bool showHeader = !Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1;
                
                if (model.IsExpanded(model.Groups[section - 1]))
                {
                    return model.Groups[section - 1].Pawns.Count + (showHeader ? 1 : 0);
                } 
                else
                {
                    return (showHeader ? 1 : 0);
                }
            }
        }

        public ICTableGridRow rowAt(int section, int row)
        {
            if (section == 0)
            {
                return new CPawnTableHeaderRow(model);
            }
            else
            {
                var weakThis = new Verse.WeakReference<PawnTableGroupedView>(this);
                bool showHeader = !Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1;

                var group = model.Groups[section - 1];
                if (showHeader && row == 0)
                {
                    var groupHeader = new CPawnTableGroupRow(group, this, model.IsExpanded(group));
                    groupHeader.Action = (group) =>
                    {
                        weakThis.Target.model.SwitchExpanded(group);
                    };
                    return groupHeader;
                }
                else
                {
                    var pawn = group.Pawns[row - (showHeader ? 1 : 0)];
                    return new CPawnTablePawnRow(model.table, group, pawn);
                }
            }
        }

        public void OnGUI(Vector2 position, int magic)
        {
            host.InRect = new Rect((int)position.x, (int)position.y, (int)model.Table.cachedSize.x, (int)model.Table.cachedSize.y);
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

        public bool canMergeRows(int column)
        {
#if rw_1_4_or_later
            return model.Table.Columns[column].groupable;            
#else
            return false;
#endif
        }
    }

}
