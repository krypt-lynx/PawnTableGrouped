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
        public PawnTable Table
        {
            get => table.Target; 
            set
            {
                table = new Verse.WeakReference<PawnTable>(value);
            }
        }

        PawnTableAccessor accessor;


        PawnTableGroupedModel model;
        PawnTableGroupedGUI view;


        public PawnTableGroupedImpl(PawnTable table, PawnTableDef def)
        {
            this.Table = table;
            //this.def = def;

            accessor = new PawnTableAccessor(table);


            model = new PawnTableGroupedModel(table, accessor, def);
            view = new PawnTableGroupedGUI(model);

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
            if (NeedUpdateViews)
            {
                view.PopulateList();
                model.DoGroupsStateChanged();
                NeedUpdateViews = false;
            }

            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            var magic = NumbersBridge.IsNumbersTable(Table) ? NumbersBridge.ReorderableGroup(Table) : 0;
            accessor.RecacheIfDirty();

            view.OnGUI(position, magic);
        }

        public virtual void RecacheIfDirty()
        {  // todo: move to model

            if (!accessor.dirty)
            {
                return;
            }
            accessor.dirty = false;
            // $"PawnTableGroupedImpl RecacheIfDirty".Log();

            model.RecacheColumnResolvers();
            accessor.RecachePawns();
            model.RecacheGroups();

            accessor.RecacheRowHeights();
            accessor.cachedHeaderHeight = accessor.CalculateHeaderHeight();
            accessor.cachedHeightNoScrollbar = CalculateTotalRequiredHeight();
            accessor.RecacheSize();

            var size = accessor.cachedSize;
            accessor.cachedSize = new Vector2(Mathf.Min(size.x, accessor.maxTableWidth - Metrics.TableLeftMargin), size.y);


            accessor.RecacheColumnWidths();
            //var columnWidths = accessor.cachedColumnWidths; 

            AdjastTableWidth();

            accessor.RecacheLookTargets();

            NeedUpdateViews = true;
        }

        private void AdjastTableWidth()
        {
            float totalColumnsWidth;
            var fits = UpdateColumnWidths(out totalColumnsWidth);
            view.SetInnerWidth(totalColumnsWidth + Metrics.TableLeftMargin);

            var size = accessor.cachedSize;
            accessor.cachedSize = new Vector2(size.x + Metrics.TableLeftMargin,
                Mathf.Min(size.y + (fits ? 0 : Metrics.ScrollBar), accessor.maxTableHeight)); // expand table for collapse indicator and horizontal scrollbar
        }

        private bool UpdateColumnWidths(out float width)
        {
            var cachedColumnWidths = accessor.cachedColumnWidths;
            float optimalWidth = accessor.cachedSize.x - Metrics.ScrollBar;

            if (model.AllowHScroll)
            {
                float minWidth = 0;
                for (int i = 0; i < model.def.columns.Count; i++)
                {
                    if (!model.def.columns[i].ignoreWhenCalculatingOptimalTableSize)
                    {
                        var minColumnWidth = accessor.GetMinWidth(model.def.columns[i]);
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
