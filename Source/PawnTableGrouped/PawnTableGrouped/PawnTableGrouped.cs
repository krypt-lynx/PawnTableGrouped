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
        PawnTableWrapper table;

        PawnTableGroupedModel model;
        PawnTableGroupedView view;


        public PawnTableGroupedImpl(PawnTableWrapper table, PawnTableDef def)
        {
            this.table = table;
            //this.def = def;


            model = new PawnTableGroupedModel(table, def);
            view = new PawnTableGroupedView(model);

            new EventBusListener<PawnTableGroupedImpl, PawnTableInvalidateMessage>(this, (x, sender, args) =>
            {
                x.table.SetDirty();
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

            var magic = NumbersBridge.IsNumbersTable(table.Table) ? NumbersBridge.ReorderableGroup(table.Table) : 0;
            //accessor.RecacheIfDirty();

            view.OnGUI(position, magic);
        }

        public virtual void RecacheIfDirty()
        {  // todo: move to model

            if (!table.dirty)
            {
                return;
            }
            table.dirty = false;
            // $"PawnTableGroupedImpl RecacheIfDirty".Log();

            model.RecacheColumnResolvers();
            table.RecachePawns();
            model.RecacheGroups();

            table.RecacheRowHeights();
            table.cachedHeaderHeight = table.CalculateHeaderHeight();
            table.cachedHeightNoScrollbar = CalculateTotalRequiredHeight();
            table.RecacheSize();

            var size = table.cachedSize;
            table.cachedSize = new Vector2(Mathf.Min(size.x, table.maxTableWidth - Metrics.TableLeftMargin), size.y);


            table.RecacheColumnWidths();
            //var columnWidths = accessor.cachedColumnWidths; 

            AdjastTableWidth();

            table.RecacheLookTargets();

            NeedUpdateViews = true;
        }

        internal void SetOwnerWindow(MainTabWindow_PawnTableWrapper ownerWindow)
        {
            model.OnChanged = () => ownerWindow.SetDirty();
        }

        private void AdjastTableWidth()
        {
            float totalColumnsWidth;
            var fits = UpdateColumnWidths(out totalColumnsWidth);
            //view.SetInnerWidth(totalColumnsWidth + Metrics.TableLeftMargin);

            var size = table.cachedSize;
            table.cachedSize = new Vector2(size.x + Metrics.TableLeftMargin,
                Mathf.Min(size.y + (fits ? 0 : Metrics.ScrollBar), table.maxTableHeight)); // expand table for collapse indicator and horizontal scrollbar
        }

        private bool UpdateColumnWidths(out float width)
        {
            var cachedColumnWidths = table.cachedColumnWidths;
            float optimalWidth = table.cachedSize.x - Metrics.ScrollBar;

            if (model.AllowHScroll)
            {
                float minWidth = 0;
                for (int i = 0; i < model.def.columns.Count; i++)
                {
                    if (!model.def.columns[i].ignoreWhenCalculatingOptimalTableSize)
                    {
                        var minColumnWidth = table.GetMinWidth(model.def.columns[i]);
                        minWidth += minColumnWidth;
                        cachedColumnWidths[i] = Mathf.Max(cachedColumnWidths[i], minColumnWidth);
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
