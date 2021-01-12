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
//    class CGroupRow : CListingRow
//    {
//        List<CListingRow> subrows;
//        Action<bool> GetCollapsed;
//    }

    public class CPawnTableRow : CListingRow // CElementHost
    {

    }


    public class CPawnTable : CElement 
    {
        Rect innerRect;
        /// <summary>
        /// Scroll Location
        /// </summary>
        public Vector2 ScrollPosition = Vector2.zero;

        List<CPawnTableRow> rows = new List<CPawnTableRow>();
        public IReadOnlyList<CPawnTableRow> Rows { get => rows; }

        public EdgeInsets Margin = EdgeInsets.Zero;

        private CGuiRoot background = new CGuiRoot();
        public CElement Background { get => background; }

        //float contentHeight = 0;

        public float InnerWidth;

        public override CElement hitTest(Vector2 point)
        {
            if (userInteractionEnabled && Bounds.Contains(point))
            {
                var listPoint = point - this.Bounds.position + this.ScrollPosition;

                var sub = base.hitTest(point);
                if (sub != this)
                {
                    return sub;
                }

                foreach (var row in rows)
                {
                    var element = row.hitTest(listPoint);
                    if (element != null)
                    {
                        return element;
                    }
                }

                return this;
            }
            else
            {
                return null;
            }
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            var skin = GUI.skin.verticalScrollbar;

            float y = Margin.Top;
            float w = InnerWidth; // BoundsRounded.width - (skin.fixedWidth + skin.margin.left);
            foreach (var row in rows)
            {
                row.InRect = new Rect(Margin.Left, y, w - Margin.Left - Margin.Right, 0);
                row.UpdateLayoutIfNeeded();
                y += row.Bounds.height;
            }
            y += Margin.Bottom;

            innerRect = new Rect(0, 0, w, y).GUIRoundedPreserveOrigin();
            background.InRect = innerRect;
            background.UpdateLayoutIfNeeded();
        }

        public override void DoContent()
        {
            base.DoContent();
           // bool showScrollBar = true;

            //showScrollBar = true;
            Widgets.BeginScrollView(BoundsRounded, ref ScrollPosition, innerRect, true);

            DoScrollContent();

            Widgets.EndScrollView();
        }

        public virtual void DoScrollContent()
        {
            Background.DoElementContent();
            var windowMin = ScrollPosition.y;
            var windowMax = ScrollPosition.y + this.BoundsRounded.height;

            foreach (var element in rows)
            {
                if ((element.BoundsRounded.yMax > windowMin) && (element.BoundsRounded.yMin < windowMax))
                {
                    element.DoElementContent();
                }
            }
        }

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
    }
}
