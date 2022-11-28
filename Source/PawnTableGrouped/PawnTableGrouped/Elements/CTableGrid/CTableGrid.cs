using HarmonyLib;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using PawnTableGrouped.TableGrid.Collections;
using PawnTableGrouped.TableGrid;

namespace PawnTableGrouped
{
    public class CTableGrid : CElement
    {
        Rect tableBodyOuterRect;

        Rect scrollOuterRectClipped;

        float scrollInnerWidth;
        float scrollInnerHeight;

        float vScrollMax;
        float hScrollMax;

        float fixedWidth;
        float fixedHeight;

        float scrollBodyWidth;
        float scrollBodyHeight;

        Rect fixedPanelRect;
        Rect columnsPanelRect;
        Rect namesPanelRect;

        Rect hScrollZoneRect;


        Verse.WeakReference<ICTableGridDataSource> dataSource;
        public ICTableGridDataSource DataSource
        {
            get => dataSource.Target;
            set
            {
                dataSource = new Verse.WeakReference<ICTableGridDataSource>(value);
                Invalidate();
                sections = null;
                columns = null;
            }
        }

        public void Invalidate()
        {
            sections?.Invalidate();
            columns?.Invalidate();
        }

        CTableGridSectionsCollection sections = null;
        CTableGridSectionsCollection Sections
        {
            get
            {
                if (sections == null)
                {
                    sections = new CTableGridSectionsCollection(DataSource);
                }
                return sections;
            }
        }

        CTableGridColumnsCollection columns = null;
        CTableGridColumnsCollection Columns
        {
            get
            {
                if (columns == null)
                {
                    columns = new CTableGridColumnsCollection(DataSource);
                }
                return columns;
            }
        }

        public bool allowHScroll = false;


        bool vScrollVisible = false;
        bool hScrollVisible = false;
        Rect hScrollRect;
        Rect vScrollRect;
        float hScrollPosition = 0;
        float vScrollPosition = 0;


        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            var vBarWidth = GUI.skin.verticalScrollbar.fixedWidth + GUI.skin.verticalScrollbar.margin.left;
            var hBarHeight = GUI.skin.horizontalScrollbar.fixedHeight + GUI.skin.horizontalScrollbar.margin.top;

            if (Sections.Count == 0 || Columns.TotalSize() == 0 || Columns.Count == 0)
            {
                vScrollVisible = false;
                hScrollVisible = false;

                return;
            }

            hScrollVisible = allowHScroll && (Columns.TotalSize() > Bounds.width - vBarWidth);
            var hScrollHeight = hScrollVisible ? hBarHeight : 0;

            vScrollVisible = Sections.TotalSize() > Bounds.height - hScrollHeight;
            var vScrollWidth = vBarWidth;

            fixedWidth = Columns.IntegralSizeOf(1);
            fixedHeight = Sections[0].IntegralSizeOf(1);

            scrollBodyWidth = Bounds.width - fixedWidth - vScrollWidth;
            scrollBodyHeight = Bounds.height - fixedHeight - hScrollHeight;

            if (vScrollVisible)
            {
                vScrollRect = Rect.MinMaxRect(
                    Bounds.xMax - vBarWidth,
                    Bounds.yMin + fixedHeight,
                    Bounds.xMax,
                    Bounds.yMax - hScrollHeight);
            }

            if (hScrollVisible)
            {
                hScrollRect = Rect.MinMaxRect(
                    Bounds.xMin + fixedWidth,
                    Bounds.yMax - hBarHeight,
                    Bounds.xMax - vScrollWidth,
                    Bounds.yMax);
            }

            fixedPanelRect = new Rect(Bounds.xMin, Bounds.yMin, fixedWidth, fixedHeight);
            columnsPanelRect = new Rect(Bounds.xMin + fixedWidth, Bounds.yMin, scrollBodyWidth, fixedHeight);

            tableBodyOuterRect = new Rect(Bounds.xMin, Bounds.yMin + fixedHeight, fixedWidth + scrollBodyWidth, scrollBodyHeight);

            namesPanelRect = new Rect(0, fixedHeight, fixedWidth, scrollBodyHeight);
            namesPanelRect = new Rect(0, 0, fixedWidth, scrollBodyHeight);

            hScrollZoneRect = Rect.MinMaxRect(Bounds.xMin, Bounds.yMin, Bounds.xMax, Bounds.yMin + fixedHeight);

            scrollInnerWidth = Columns.TotalSize() - fixedWidth;
            scrollInnerHeight = Sections.TotalSize() - fixedHeight;

            scrollOuterRectClipped = new Rect(fixedWidth, 0, scrollBodyWidth, scrollBodyHeight);

            vScrollMax = scrollInnerWidth - scrollOuterRectClipped.x;
            hScrollMax = scrollInnerHeight - scrollOuterRectClipped.y;
        }

     
        int controlID = 0;
        private void ScrollBegin()
        {            
            controlID = GUIUtility.GetControlID(pawnTableScrollHash, FocusType.Passive, Bounds);            
        }

        private void ScrollEnd()
        {
            EventType typeForControl = Event.current.GetTypeForControl(controlID);

            if (typeForControl == EventType.ScrollWheel)
            {
                GUIUtility.hotControl = controlID;

                var delta = Event.current.delta * 20;

                if (Event.current.shift)
                {
                    hScrollPosition = Mathf.Clamp(hScrollPosition + delta.y, 0, hScrollMax);
                }
                else if (Event.current.alt)
                {
                    vScrollPosition = Mathf.Clamp(vScrollPosition + delta.y, 0, vScrollMax);
                }
                else if (hScrollZoneRect.Contains(Event.current.mousePosition) ||
                    (hScrollVisible && hScrollRect.Contains(Event.current.mousePosition)))
                {
                    hScrollPosition = Mathf.Clamp(hScrollPosition + delta.y, 0, hScrollMax);
                }
                else
                {
                    hScrollPosition = Mathf.Clamp(hScrollPosition + delta.x, 0, hScrollMax);
                    vScrollPosition = Mathf.Clamp(vScrollPosition + delta.y, 0, vScrollMax);
                }

                Event.current.Use();
            }
        }


        private static readonly int pawnTableScrollHash = "CTableGrid".GetHashCode();

        public override void DoContent()
        {
            base.DoContent();

            // scroll bars
            DoScrollbars();

            ScrollBegin();

            // Left-Top fixed corner cell
            DoFixedCell();

            // Column headers
            DoColumnHeaders();

            GUI.BeginGroup(tableBodyOuterRect);
            int highlightedSectionIndex = -1;
            int highlightedRowIndex = -1;

            // row backgrounds
            DoGridBackground(ref highlightedSectionIndex, ref highlightedRowIndex);

            // fixed names column
            DoFixedColumn(highlightedSectionIndex, highlightedRowIndex);

            // scrollable table cells
            DoTableContent(highlightedSectionIndex, highlightedRowIndex);

            // row overlays
            DoTableOverlays();

            GUI.EndGroup();

            ScrollEnd();
        }

        private void DoScrollbars()
        {
            if (hScrollVisible)
            {
                hScrollPosition = GUI.HorizontalScrollbar(hScrollRect, hScrollPosition, scrollOuterRectClipped.width, 0, scrollInnerWidth);
            }
            else
            {
                hScrollPosition = 0;
            }

            if (vScrollVisible)
            {
                vScrollPosition = GUI.VerticalScrollbar(vScrollRect, vScrollPosition, scrollOuterRectClipped.height, 0, scrollInnerHeight);
            }
            else
            {
                vScrollPosition = 0;
            }
        }

        private void DoFixedCell()
        {
            Sections[0][0].DoCell(fixedPanelRect, 0, false, false);
        }

        private void DoColumnHeaders()
        {
            GUI.BeginGroup(columnsPanelRect);

            var minX = 0;
            var maxX = columnsPanelRect.width;

            for (int columnIndex = 1; columnIndex < Columns.Count; columnIndex++)
            {
                var offsetX = hScrollPosition + fixedWidth;
                var cellXMin = Columns.IntegralSizeOf(columnIndex) - offsetX;
                var cellXMax = Columns.IntegralSizeOf(columnIndex + 1) - offsetX;

                if (cellXMin < maxX &&
                    cellXMax > minX)
                {
                    Sections[0][0].DoCell(
                        Rect.MinMaxRect(cellXMin, 0, cellXMax, fixedHeight),
                        columnIndex, false, false);
                }
            }

            GUI.EndGroup();
        }

        private void DoTableOverlays()
        {
            var minY = 0;
            var maxY = scrollInnerHeight;
            var offsetY = fixedHeight + vScrollPosition;

            for (int sectionIndex = 1; sectionIndex < Sections.Count; sectionIndex++)
            {
                var section = Sections[sectionIndex];
                var sectionMinY = Sections.IntegralSizeOf(sectionIndex) - offsetY;
                var sectionMaxY = Sections.IntegralSizeOf(sectionIndex + 1) - offsetY;

                if (sectionMinY < maxY &&
                    sectionMaxY > minY)
                {
                    for (int rowIndex = 0; rowIndex < section.Count; rowIndex++)
                    {
                        var cellYMin = sectionMinY + section.IntegralSizeOf(rowIndex);
                        var cellYMax = sectionMinY + section.IntegralSizeOf(rowIndex + 1);

                        if (cellYMin < maxY &&
                            cellYMax > minY)
                        {
                            Sections[sectionIndex][rowIndex].DoOverlay(Rect.MinMaxRect(0, cellYMin, tableBodyOuterRect.width, cellYMax));
                        }
                    }
                }
            }
        }

        private void DoTableContent(int highlightedSectionIndex, int highlightedRowIndex)
        {
            GUI.BeginGroup(scrollOuterRectClipped);

            var minX = 0;
            var minY = 0;
            var maxX = columnsPanelRect.width;
            var maxY = scrollInnerHeight;

            var offsetY = vScrollPosition + fixedHeight;
            var offsetX = hScrollPosition + fixedWidth;

            // enumeration sections
            for (int sectionIndex = 1; sectionIndex < Sections.Count; sectionIndex++)
            {
                var section = Sections[sectionIndex];
                var sectionMinY = Sections.IntegralSizeOf(sectionIndex) - offsetY;
                var sectionMaxY = Sections.IntegralSizeOf(sectionIndex + 1) - offsetY;

                // if section is visible
                if (sectionMinY < maxY &&
                    sectionMaxY > minY)
                {
                    // enumerating columns
                    for (int columnIndex = 1; columnIndex < Columns.Count; columnIndex++)
                    {
                        var cellXMin = Columns.IntegralSizeOf(columnIndex) - offsetX;
                        var cellXMax = Columns.IntegralSizeOf(columnIndex + 1) - offsetX;

                        // is column visible
                        if (cellXMin < maxX &&
                            cellXMax > minX)
                        {

                            // enumerating rows
                            var firstCombined = -1;
                            var highlightedCombined = false;
                            for (int rowIndex = 0; rowIndex < section.Count; rowIndex++)
                            {
                                var row = section[rowIndex];

                                // if can combine rows in this column (mechanitor column for mechs, for example)
                                if (DataSource.canMergeRows(columnIndex))
                                {
                                    // do rows merging
                                    bool willCombine = false;
                                    if (rowIndex != section.Count - 1)
                                    {
                                        willCombine = row.CanCombineWith(section[rowIndex + 1], columnIndex);
                                    }

                                    highlightedCombined |= (highlightedSectionIndex == sectionIndex && highlightedRowIndex == rowIndex);
                                    if (willCombine)
                                    {
                                        if (firstCombined == -1)
                                        {
                                            firstCombined = rowIndex;
                                        }
                                    }
                                    else
                                    {
                                        var beginIndex = firstCombined == -1 ? rowIndex : firstCombined;
                                        var cellYMin = sectionMinY + section.IntegralSizeOf(beginIndex);
                                        var cellYMax = sectionMinY + section.IntegralSizeOf(rowIndex + 1);

                                        // render cell if visible
                                        if (cellYMin < maxY &&
                                            cellYMax > minY)
                                        {
                                            row.DoCell(Rect.MinMaxRect(cellXMin, cellYMin, cellXMax, cellYMax),
                                                columnIndex,
                                                highlightedCombined,
                                                firstCombined != -1);
                                        }
                                        firstCombined = -1;
                                        highlightedCombined = false;
                                    }
                                }
                                else
                                {
                                    // it can't, use short version logic
                                    var cellYMin = sectionMinY + section.IntegralSizeOf(rowIndex);
                                    var cellYMax = sectionMinY + section.IntegralSizeOf(rowIndex + 1);

                                    // render cell if visible
                                    if (cellYMin < maxY &&
                                        cellYMax > minY)
                                    {
                                        row.DoCell(
                                            Rect.MinMaxRect(cellXMin, cellYMin, cellXMax, cellYMax),
                                            columnIndex,
                                            highlightedSectionIndex == sectionIndex && highlightedRowIndex == rowIndex,
                                            false);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            GUI.EndGroup();
        }

        private void DoFixedColumn(int highlightedSectionIndex, int highlightedRowIndex)
        {
            GUI.BeginGroup(namesPanelRect);

            var minY = 0;
            var maxY = scrollInnerHeight;

            var offsetY = fixedHeight + vScrollPosition;
            for (int sectionIndex = 1; sectionIndex < Sections.Count; sectionIndex++)
            {
                var section = Sections[sectionIndex];
                var sectionMinY = Sections.IntegralSizeOf(sectionIndex) - offsetY;
                var sectionMaxY = Sections.IntegralSizeOf(sectionIndex + 1) - offsetY;

                if (sectionMinY < maxY &&
                    sectionMaxY > minY)
                {
                    for (int rowIndex = 0; rowIndex < section.Count; rowIndex++)
                    {
                        var cellYMin = sectionMinY + section.IntegralSizeOf(rowIndex);
                        var cellYMax = sectionMinY + section.IntegralSizeOf(rowIndex + 1);

                        if (cellYMin < maxY &&
                            cellYMax > minY)
                        {
                            Sections[sectionIndex][rowIndex].DoCell(
                                Rect.MinMaxRect(0, cellYMin, fixedWidth, cellYMax),
                                0,
                                highlightedSectionIndex == sectionIndex && highlightedRowIndex == rowIndex,
                                false);
                        }
                    }
                }
            }

            GUI.EndGroup();
        }

        private void DoGridBackground(ref int highlightedSectionIndex, ref int highlightedRowIndex)
        {
            var minY = 0;
            var maxY = scrollInnerHeight;
            var offsetY = fixedHeight + vScrollPosition;

            for (int sectionIndex = 1; sectionIndex < Sections.Count; sectionIndex++)
            {
                var section = Sections[sectionIndex];
                var sectionMinY = Sections.IntegralSizeOf(sectionIndex) - offsetY;
                var sectionMaxY = Sections.IntegralSizeOf(sectionIndex + 1) - offsetY;

                if (sectionMinY < maxY &&
                    sectionMaxY > minY)
                {
                    for (int rowIndex = 0; rowIndex < section.Count; rowIndex++)
                    {
                        var cellYMin = sectionMinY + section.IntegralSizeOf(rowIndex);
                        var cellYMax = sectionMinY + section.IntegralSizeOf(rowIndex + 1);

                        if (cellYMin < maxY &&
                            cellYMax > minY)
                        {
                            var bgRect = Rect.MinMaxRect(0, cellYMin, tableBodyOuterRect.width, cellYMax);
                            if (Mouse.IsOver(bgRect))
                            {
                                highlightedSectionIndex = sectionIndex;
                                highlightedRowIndex = rowIndex;
                            }
                            Sections[sectionIndex][rowIndex].DoBackground(bgRect);
                        }
                    }
                }
            }
        }
    }

}
