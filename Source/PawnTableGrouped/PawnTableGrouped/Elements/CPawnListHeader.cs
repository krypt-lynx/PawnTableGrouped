﻿using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PawnTableGrouped
{
    class CPawnListHeader : CRowSegment
    {
        private RangeInt columnsRange;
        private bool doLeftOffset;

        private PawnTableGroupedModel model;
        public CPawnListHeader(PawnTableGroupedModel model, RangeInt columnsRange)
        {
            this.model = model;
            this.columnsRange = columnsRange;
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(model.accessor.cachedHeightNoScrollbar, model.accessor.cachedHeaderHeight);
        }

        public override void DoContent()
        {
            base.DoContent();

            var columns = model.Table.ColumnsListForReading;
            var cachedColumnWidths = model.accessor.cachedColumnWidths;
    

            int x = (int)(BoundsRounded.xMin + (doLeftOffset ? Metrics.TableLeftMargin : 0));

            int start = Mathf.Max(0, columnsRange.start);
            int end = Mathf.Min(columns.Count, columnsRange.end);
            for (int columnIndex = start; columnIndex < end; columnIndex++)
            {
                int columnWidth;
                if (columnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(BoundsRounded.width - x);
                }
                else
                {
                    columnWidth = (int)model.accessor.cachedColumnWidths[columnIndex];
                }

                if (x + columnWidth > xScrollOffset && x <= xScrollOffset + visibleRectWidth)
                {
                    Rect rect = new Rect(x, BoundsRounded.yMin, columnWidth, BoundsRounded.height);

                    NumbersWrapper.CallReorderableWidget(model.NumbersMagic, rect);
                    columns[columnIndex].Worker.DoHeader(rect, model.Table);
                }

                x += columnWidth;
            }

        }
    }

}