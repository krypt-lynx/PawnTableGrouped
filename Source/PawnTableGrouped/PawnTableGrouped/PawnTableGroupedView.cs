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

        public int NumberOfColumns()
        {
            return model.Table.Columns().Count;
        }

        public float WidthForColumn(int column)
        {
            return model.Table.GetCachedColumnWidths()[column] + (column == 0 ? Metrics.TableLeftMargin : 0);
        }

        public int NumberOfSections()
        {
            return model.Groups.Count + 1;
        }

        public ICTableGridSection SectionAt(int section)
        {
            if (section == 0)
            {
                return new CTableGridHeaderSection(model);
            } 
            else
            {
                var fixedWidth = NumberOfColumns() > 0 ? WidthForColumn(0) : 0;
                return new CTableGridGroupSection(model, model.Groups[section - 1], fixedWidth);
            }
        }

        bool highlight = false;

        public ICTableGridColumn ColumnAt(int column)
        {
            // a hack (of my own code), assuming columns enumerated from left to right continiusly (which is true for the moment)
            if (KnownMods.WorkTab.IsActive &&
                KnownMods.WorkTab.IsWorkTabWindow(model.Window) &&
                KnownMods.WorkTab.Expanded(model.Window))
            {
                var workerType = model.Table.Columns()[column].workerClass;
                if (workerType  == KnownMods.WorkTab.WorkTypeWorkerType)
                {
                    highlight = !highlight;
                } 
                else if (workerType != KnownMods.WorkTab.WorkGiverWorkerType)
                {
                    highlight = false;
                }
            }
            else
            {
                highlight = false;
            }

            return new CPawnTableColumn(highlight);
        }

        public void OnGUI(Vector2 position, int magic)
        {
            host.InRect = new Rect((int)position.x, (int)position.y, (int)model.Table.GetCachedSize().x, (int)model.Table.GetCachedSize().y);
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
