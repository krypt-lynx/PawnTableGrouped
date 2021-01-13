﻿using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class CRowSegment : CListingRow
    {
        Verse.WeakReference<CPawnTableRow> row = null;
        public CPawnTableRow Row
        {
            get
            {
                return row?.Target;
            }
            set
            {
                row = new Verse.WeakReference<CPawnTableRow>(value);
            }
        }

        public virtual float xScrollOffset { get; set; } = 0;
        public virtual float visibleRectWidth { get; set; } = 0;
    }

    public class CPawnTableRow 
    {
        public Verse.WeakReference<CPawnTable> owner = null;
        public CPawnTable Owner
        {
            get
            {
                return owner?.Target;
            }
            set
            {
                owner = new Verse.WeakReference<CPawnTable>(value);
            }
        }

        public CRowSegment fixed_;
        public CRowSegment Fixed {
            get => fixed_;
            set
            {
                fixed_ = value;
                fixed_.Row = this;
            }
        }

        public CRowSegment row_;
        public CRowSegment Row
        {
            get => row_;
            set
            {
                row_ = value;
                row_.Row = this;
            }
        }

        public virtual float xScrollOffset
        {
            get
            {
                return 0;
            }
            set
            {
                if (Fixed != null)
                {
                    Fixed.xScrollOffset = value;
                }
                if (Row != null)
                {
                    Row.xScrollOffset = value;
                }
            }
        }
        public virtual float visibleRectWidth
        {
            get
            {
                return 0;
            }
            set
            {
                if (Fixed != null)
                {
                    Fixed.visibleRectWidth = value;
                }
                if (Row != null)
                {
                    Row.visibleRectWidth = value;
                }
            }
        }
    }


    public class CPawnTable : CElement 
    {
        Rect vScrollViewOuterRect;
        Rect vScrollViewInnerRect;
        Rect hScrollClipRect;
        Rect hScrollInnerRect;
        /// <summary>
        /// Scroll Location
        /// </summary>
        public Vector2 OuterScrollPosition = Vector2.zero;

        public CPawnTableRow TableHeader = null;
        List<CPawnTableRow> rows = new List<CPawnTableRow>();
        public IReadOnlyList<CPawnTableRow> Rows { get => rows; }

        public float FixedSegmentWidth { get; internal set; }


        public float InnerWidth;


        Rect hScrollRect;
        float hScrollPosition = 0;
        bool hScrollVisible = false;

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            var vBarWidth = GUI.skin.verticalScrollbar.fixedWidth + GUI.skin.verticalScrollbar.margin.left;
            var hBarHeight = GUI.skin.horizontalScrollbar.fixedHeight + GUI.skin.horizontalScrollbar.margin.top;

            float y = 0;
            foreach (var row in rows)
            {
                if (row.Fixed != null)
                {
                    row.Fixed.InRect = new Rect(0, y, FixedSegmentWidth, 0);
                    row.Fixed.UpdateLayoutIfNeeded();
                }
                if (row.Row != null)
                {
                    row.Row.InRect = new Rect(0, y, InnerWidth - FixedSegmentWidth, 0);
                    row.Row.UpdateLayoutIfNeeded();
                }
                y += Mathf.Max(row.Fixed?.Bounds.height ?? 0, row.Row?.Bounds.height ?? 0);
            }

            hScrollVisible = InnerWidth > Bounds.width - vBarWidth;

            vScrollViewOuterRect = Rect.MinMaxRect(Bounds.xMin, Bounds.yMin, Bounds.xMax, Bounds.yMax - (hScrollVisible ? hBarHeight : 0));

            vScrollViewInnerRect = new Rect(0, 0, Bounds.width - vBarWidth, y).GUIRoundedPreserveOrigin();
            hScrollClipRect = new Rect(FixedSegmentWidth, 0, Bounds.width - vBarWidth - FixedSegmentWidth, y);
            hScrollInnerRect = new Rect(0, 0, InnerWidth - FixedSegmentWidth, y);
            hScrollRect = Rect.MinMaxRect(Bounds.xMin + FixedSegmentWidth, Bounds.yMax - hBarHeight, Bounds.xMax - vBarWidth, Bounds.yMax);
        }


        public override void DoContent()
        {
            base.DoContent();

            if (hScrollVisible)
            {
                hScrollPosition = GUI.HorizontalScrollbar(hScrollRect, hScrollPosition, hScrollClipRect.width, 0, hScrollInnerRect.width);
            }
            else
            {
                hScrollPosition = 0;
            }

            Widgets.BeginScrollView(vScrollViewOuterRect, ref OuterScrollPosition, vScrollViewInnerRect, true);

            var windowYMin = OuterScrollPosition.y;
            var windowYMax = OuterScrollPosition.y + this.BoundsRounded.height;

            GUI.BeginClip(hScrollClipRect);
            var rect = hScrollInnerRect;
            rect.x = rect.x - hScrollPosition;
            GUI.BeginGroup(rect);

            foreach (var row in rows)
            {
                if ((row.Row.BoundsRounded.yMax > windowYMin) && (row.Row.BoundsRounded.yMin < windowYMax))
                {
                    row.xScrollOffset = hScrollPosition;
                    row.visibleRectWidth = hScrollClipRect.width;
                    row.Row?.DoElementContent();
                }
            }

            GUI.EndGroup();
            GUI.EndClip();

            foreach (var row in rows)
            {
                if ((row.Row.BoundsRounded.yMax > windowYMin) && (row.Row.BoundsRounded.yMin < windowYMax))
                {
                    row.xScrollOffset = 0;
                    row.visibleRectWidth = FixedSegmentWidth;
                    row.Fixed?.DoElementContent();
                }
            }

            Widgets.EndScrollView();
        }

        #region rows manipulations

        /// <summary>
        /// Append a row to CListView.
        /// </summary>
        /// <param name="row">the row</param>
        /// <returns>the row</returns>
        /// <remarks>Row must not be added in this or other CListView</remarks>
        public CPawnTableRow AppendRow(CPawnTableRow row)
        {
            if (row.Owner != null)
            {
                throw new InvalidOperationException($"{row} is already added to {row.Owner}");
            }

            row.Owner = this;
            rows.Add(row);
            SetNeedsUpdateLayout();
            return row;
        }

        /// <summary>
        /// Insert a row to CListView at index.
        /// </summary>
        /// <param name="index">index to insert the row at</param>
        /// <param name="row">the row</param>
        /// <returns>the row</returns>
        /// <remarks>Row must not be added in this or other CListView</remarks>
        public CPawnTableRow InsertRow(int index, CPawnTableRow row)
        {
            if (row.Owner != null)
            {
                throw new InvalidOperationException($"{row} is already added to {row.Owner}");
            }

            row.Owner = this;
            rows.Insert(index, row);
            SetNeedsUpdateLayout();
            return row;
        }

        /// <summary>
        /// Removes the row from CListView
        /// </summary>
        /// <param name="row">the row</param>
        /// <returns>true if row was successfully removed</returns>
        public bool RemoveRow(CPawnTableRow row)
        {
            bool result;
            if (result = rows.Remove(row))
            {
                row.Owner = null;
            }
            SetNeedsUpdateLayout();
            return result;
        }

        /// <summary>
        /// Removes the row at index from CListView
        /// </summary>
        /// <param name="row">index of row to remove</param>
        /// <returns>the removed row</returns>
        public CPawnTableRow RemoveRowAt(int index)
        {
            var row = rows[index];
            rows.RemoveAt(index);
            row.Owner = null;
            SetNeedsUpdateLayout();
            return row;
        }

        /// <summary>
        /// Moves row to index
        /// </summary>
        /// <param name="row">the row</param>
        /// <param name="index">new index</param>
        public void MoveRowTo(CPawnTableRow row, int index)
        {
            rows.Remove(row);
            rows.Insert(index, row);
            SetNeedsUpdateLayout();
        }

        /// <summary>
        /// Returns index of the row
        /// </summary>
        /// <param name="row">the row</param>
        /// <returns>The zero-based index of the row, if found; otherwise, -1.</returns>
        public int IndexOfRow(CPawnTableRow row)
        {
            return rows.IndexOf(row);
        }

        /// <summary>
        /// Removes all rows
        /// </summary>
        public void ClearRows()
        {
            foreach (var row in rows)
            {
                row.Owner = null;
            }
            rows.Clear();
        }

        #endregion
    }
}