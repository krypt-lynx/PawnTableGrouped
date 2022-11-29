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
        public const float ScrollBar = 16;

        public const float GroupHeaderOpacityIcon = 0.4f;
        public const float GroupHeaderOpacityText = 0.6f;
        public readonly static Color GroupHeaderOpacityIconColor = new Color(1, 1, 1, GroupHeaderOpacityIcon);
        public readonly static Color GroupHeaderOpacityColor = new Color(1, 1, 1, GroupHeaderOpacityText);

        public const float PawnTableFooterHeight = 30;
    }


    public class PawnTableGroupedImpl
    {
        Verse.WeakReference<PawnTable> table;        
        PawnTableGroupedModel model;
        PawnTableGroupedView view;

        PawnTable Table => table.Target;


        public PawnTableGroupedImpl(PawnTable table, PawnTableDef def)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
            //this.def = def;

            model = new PawnTableGroupedModel(table, def);
            view = new PawnTableGroupedView(model);

            new EventBusListener<PawnTableGroupedImpl, PawnTableInvalidateMessage>(this, (x, sender, args) =>
            {
                x.Table.SetDirty();
            });

            table.SetDirty();
        }

        public virtual float CalculateTotalRequiredHeight()
        {
            return view.CalculateTotalRequiredHeight();
        }

        public virtual void SaveData()
        {
            model.SaveData();
        }

        bool NeedUpdateViews = true;
        public virtual void PawnTableOnGUI(Vector2 position)
        {
            Table.RecacheIfDirty();

            if (NeedUpdateViews)
            {
                view.Invalidate();
                model.DoGroupsStateChanged();
                NeedUpdateViews = false;
            }

            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            var magic = KnownMods.Numbers.IsNumbersTable(Table) ? KnownMods.Numbers.ReorderableGroup(Table) : 0;
            //accessor.RecacheIfDirty();

            view.OnGUI(position, magic);
        }

        public virtual void RecacheIfDirty()
        {  // todo: move to model

            if (!Table.GetDirty())
            {
                return;
            }
            Table.SetDirty(false);
            // $"PawnTableGroupedImpl RecacheIfDirty".Log();

            model.RecacheColumnResolvers();
            Table.RecachePawns();
            model.RecacheGroups();

            Table.RecacheRowHeights();
            Table.SetCachedHeaderHeight(Table.CalculateHeaderHeight());
            Table.SetCachedHeightNoScrollbar(CalculateTotalRequiredHeight());
            Table.RecacheSize();

            var size = Table.GetCachedSize();
            Table.SetCachedSize(new Vector2(Mathf.Min(size.x, Table.GetMaxTableWidth() - Metrics.TableLeftMargin), size.y));


            Table.RecacheColumnWidths();
            //var columnWidths = accessor.cachedColumnWidths; 

            AdjastTableWidth();

            Table.RecacheLookTargets();

            NeedUpdateViews = true;
        }

        internal void SetOwnerWindow(MainTabWindow_PawnTable ownerWindow)
        {
            model.Window = ownerWindow;
        }

        private void AdjastTableWidth()
        {
            float totalColumnsWidth;
            var fits = UpdateColumnWidths(out totalColumnsWidth);
            //view.SetInnerWidth(totalColumnsWidth + Metrics.TableLeftMargin);

            var size = Table.GetCachedSize();
            Table.SetCachedSize(new Vector2(size.x + Metrics.TableLeftMargin,
                Mathf.Min(size.y + (fits ? 0 : Metrics.ScrollBar), Table.GetMaxTableHeight()))); // expand table for collapse indicator and horizontal scrollbar
        }

        private bool UpdateColumnWidths(out float width)
        {
            var cachedColumnWidths = Table.GetCachedColumnWidths();
            float optimalWidth = Table.GetCachedSize().x - Metrics.ScrollBar;

            if (model.AllowHScroll)
            {
                float minWidth = 0;
                for (int i = 0; i < model.def.columns.Count; i++)
                {
                    if (!model.def.columns[i].ignoreWhenCalculatingOptimalTableSize)
                    {
                        if (i != 0)
                        {
                            var minColumnWidth = Table.GetMinWidth(model.def.columns[i]);
                            cachedColumnWidths[i] = Mathf.Max(cachedColumnWidths[i], minColumnWidth);
                            minWidth += minColumnWidth;
                        }
                        else
                        {
                            var optColumnWidth = Table.GetOptimalWidth(model.def.columns[i]);
                            cachedColumnWidths[i] = Mathf.Max(cachedColumnWidths[i], optColumnWidth);
                            minWidth += optColumnWidth;
                        }
                    }
                }

                if (minWidth > optimalWidth)
                {
                    width = minWidth;
                    return false;
                }
                else
                {
                    width = optimalWidth;
                    return true;
                }
            }
            else
            {
                width = optimalWidth;
                return true;
            }
        }
    }

}
