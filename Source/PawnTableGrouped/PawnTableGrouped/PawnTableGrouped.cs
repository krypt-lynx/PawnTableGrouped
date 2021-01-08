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


    public class PawnTableGroupedImpl
    {
        PawnTable table; // todo: weak ref
        PawnTableAccessor accessor;
        //PawnTableDef def;


        PawnTableGroupedModel model;
        PawnTableGroupedGUI view;


        public PawnTableGroupedImpl(PawnTable table, PawnTableDef def)
        {
            this.table = table;
            //this.def = def;

            accessor = new PawnTableAccessor(table);


            model = new PawnTableGroupedModel(table, accessor, def);
            view = new PawnTableGroupedGUI(model);

            table.SetDirty();
        }

        public float CalculateTotalRequiredHeight()
        {
            return view.CalculateTotalRequiredHeight();
        }


        public void PawnTableOnGUI(Vector2 position)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            var magic = NumbersWrapper.ReorderableGroup(table);
            accessor.RecacheIfDirty();

            view.OnGUI(position, magic);
        }

        public void RecacheIfDirty()
        {  // todo: move to model

            if (!accessor.dirty)
            {
                return;
            }
            accessor.dirty = false;
            model.RecacheColumnResolvers();
            accessor.RecachePawns();
            model.RecacheGroups();

            accessor.RecacheRowHeights();
            accessor.cachedHeaderHeight = accessor.CalculateHeaderHeight();
            accessor.cachedHeightNoScrollbar = CalculateTotalRequiredHeight();
            accessor.RecacheSize();
            var oldSize = accessor.cachedSize;
            accessor.cachedSize = new Vector2(oldSize.x + Metrics.TableLeftMargin, oldSize.y); // expand table for collapse indicator
            accessor.RecacheColumnWidths();
            accessor.RecacheLookTargets();

            view.PopulateList();
            model.DoGroupsStateChanged();
        }
    }

}
