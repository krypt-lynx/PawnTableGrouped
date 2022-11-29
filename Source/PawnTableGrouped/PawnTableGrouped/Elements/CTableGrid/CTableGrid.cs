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
using System.Runtime.CompilerServices;

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

        float scrollOuterWidth;
        float scrollOuterHeight;

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

        // todo: expose in rwlayout
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float GUIScale(float value)
        {
            var scale = Prefs.UIScale;
            return ((int)(value * scale)) / scale;
        }

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

            hScrollVisible = allowHScroll && GUIScale(Columns.TotalSize()) > GUIScale(BoundsRounded.width - vBarWidth);
            var hScrollHeight = hScrollVisible ? hBarHeight : 0;

            vScrollVisible = GUIScale(Sections.TotalSize()) > GUIScale(BoundsRounded.height - hScrollHeight);
            var vScrollWidth = vBarWidth;

            fixedWidth = Columns.IntegralSizeOf(1);
            fixedHeight = Sections[0].IntegralSizeOf(1);

            scrollOuterWidth = BoundsRounded.width - fixedWidth - vScrollWidth;
            scrollOuterHeight = BoundsRounded.height - fixedHeight - hScrollHeight;

            if (vScrollVisible)
            {
                vScrollRect = Rect.MinMaxRect(
                    BoundsRounded.xMax - vBarWidth,
                    BoundsRounded.yMin + fixedHeight,
                    BoundsRounded.xMax,
                    BoundsRounded.yMax - hScrollHeight);
            }

            if (hScrollVisible)
            {
                hScrollRect = Rect.MinMaxRect(
                    BoundsRounded.xMin + fixedWidth,
                    BoundsRounded.yMax - hBarHeight,
                    BoundsRounded.xMax - vScrollWidth,
                    BoundsRounded.yMax);
            }

            fixedPanelRect = new Rect(BoundsRounded.xMin, BoundsRounded.yMin, fixedWidth, fixedHeight).GUIRounded();
            columnsPanelRect = new Rect(BoundsRounded.xMin + fixedWidth, BoundsRounded.yMin, scrollOuterWidth, fixedHeight).GUIRounded();

            tableBodyOuterRect = new Rect(BoundsRounded.xMin, BoundsRounded.yMin + fixedHeight, fixedWidth + scrollOuterWidth, scrollOuterHeight).GUIRounded();

            namesPanelRect = new Rect(0, fixedHeight, fixedWidth, scrollOuterHeight).GUIRounded();
            namesPanelRect = new Rect(0, 0, fixedWidth, scrollOuterHeight).GUIRounded();

            hScrollZoneRect = Rect.MinMaxRect(BoundsRounded.xMin, BoundsRounded.yMin, BoundsRounded.xMax, BoundsRounded.yMin + fixedHeight).GUIRounded();

            scrollInnerWidth = Columns.TotalSize() - fixedWidth;
            scrollInnerHeight = Sections.TotalSize() - fixedHeight;

            scrollOuterRectClipped = new Rect(fixedWidth, 0, scrollOuterWidth, scrollOuterHeight).GUIRounded();

            vScrollMax = scrollInnerHeight - scrollOuterHeight;
            hScrollMax = scrollInnerWidth - scrollOuterWidth;
        }


        int controlID = 0;
        private void ScrollBegin()
        {            
            controlID = GUIUtility.GetControlID(pawnTableScrollHash, FocusType.Passive, BoundsRounded);

            EventType typeForControl = Event.current.GetTypeForControl(controlID);

            if (typeForControl == EventType.ScrollWheel)
            {
                GUIUtility.hotControl = controlID;

                var delta = Event.current.delta * 20;
                if (hScrollVisible && hScrollRect.Contains(Event.current.mousePosition))
                {
                    hScrollPosition = Mathf.Clamp(hScrollPosition + delta.y, 0, hScrollMax);
                    Event.current.Use();
                }
                else if (vScrollVisible && vScrollRect.Contains(Event.current.mousePosition))
                {
                    vScrollPosition = Mathf.Clamp(vScrollPosition + delta.y, 0, vScrollMax);
                    Event.current.Use();
                }

            }
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
                else if (hScrollZoneRect.Contains(Event.current.mousePosition))
                {
                    hScrollPosition = Mathf.Clamp(hScrollPosition + delta.y, 0, hScrollMax);
                }
                else if (BoundsRounded.Contains(Event.current.mousePosition))
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
            // todo: overlays draw consistency

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
            DoGridOverlays();

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
            if (columns.Count == 0 || sections.Count == 0 || sections[0].Count == 0)
            {
                return;
            }
            // todo: call column bg/overlay?
            // todo: pass column as paramenter?
            var column = Columns[0];
            var visibleRect = new Rect(0, 0, fixedWidth, fixedHeight);
            var columnRect = Rect.MinMaxRect(0, 0, fixedWidth, fixedHeight);

            column.DoBackground(columnRect, visibleRect);
            Sections[0][0].DoCell(fixedPanelRect, 0, false, false);
            column.DoOverlay(columnRect, visibleRect);
        }

        private void DoColumnHeaders()
        {
            if (sections.Count == 0 || sections[0].Count == 0)
            {
                return;
            }

            GUI.BeginGroup(columnsPanelRect);

            var minX = 0;
            var maxX = scrollOuterWidth;

            var visibleRect = new Rect(0, 0, scrollOuterWidth, fixedHeight);

            var section = Sections[0];

            var sectionRect = Rect.MinMaxRect(-hScrollPosition, 0, scrollInnerWidth - hScrollPosition, fixedHeight);

            section.Section.DoScrollableBackground(sectionRect, visibleRect);

            for (int columnIndex = 1; columnIndex < Columns.Count; columnIndex++)
            {

                var offsetX = hScrollPosition + fixedWidth;
                var cellXMin = GUIScale(Columns.IntegralSizeOf(columnIndex) - offsetX);
                var cellXMax = GUIScale(Columns.IntegralSizeOf(columnIndex + 1) - offsetX);

                var column = Columns[columnIndex];
                var columnRect = Rect.MinMaxRect(cellXMin, 0, cellXMax, fixedHeight);
                column.DoBackground(columnRect, visibleRect);

                if (cellXMin < maxX &&
                    cellXMax > minX)
                {
                    section[0].DoCell(
                        Rect.MinMaxRect(cellXMin, 0, cellXMax, fixedHeight),
                        columnIndex, false, false);
                }

                column.DoOverlay(columnRect, visibleRect);
            }

            section.Section.DoScrollableOverlay(sectionRect, visibleRect);

            GUI.EndGroup();
        }

        private void DoGridBackground(ref int highlightedSectionIndex, ref int highlightedRowIndex)
        {
            var minY = 0;
            var maxY = scrollOuterHeight;
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

        private void DoFixedColumn(int highlightedSectionIndex, int highlightedRowIndex)
        {
            if (Columns.Count == 0)
            {
                return;
            }

            GUI.BeginGroup(namesPanelRect);

            var minY = 0;
            var maxY = scrollOuterHeight;

            var offsetY = fixedHeight + vScrollPosition;

            var visibleRect = new Rect(0, offsetY, fixedWidth, scrollOuterHeight);

            for (int sectionIndex = 1; sectionIndex < Sections.Count; sectionIndex++)
            {
                var section = Sections[sectionIndex];
                var sectionYMin = Sections.IntegralSizeOf(sectionIndex) - offsetY;
                var sectionYMax = Sections.IntegralSizeOf(sectionIndex + 1) - offsetY;

                if (sectionYMin < maxY &&
                    sectionYMax > minY)
                {
                    var column = Columns[0];
                    var columnRect = Rect.MinMaxRect(0, sectionYMin, fixedWidth, sectionYMax);
                    column.DoBackground(columnRect, visibleRect);

                    for (int rowIndex = 0; rowIndex < section.Count; rowIndex++)
                    {
                        var cellYMin = sectionYMin + section.IntegralSizeOf(rowIndex);
                        var cellYMax = sectionYMin + section.IntegralSizeOf(rowIndex + 1);

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

                    column.DoOverlay(columnRect, visibleRect);
                }
            }

            GUI.EndGroup();
        }
  
        private void DoTableContent(int highlightedSectionIndex, int highlightedRowIndex)
        {
            GUI.BeginGroup(scrollOuterRectClipped);

            var minX = 0;
            var minY = 0;
            var maxX = GUIScale(scrollOuterWidth);
            var maxY = GUIScale(scrollOuterHeight);

            var offsetX = hScrollPosition + fixedWidth;
            var offsetY = vScrollPosition + fixedHeight;

            var visibleRect = new Rect(0, 0, scrollOuterWidth, scrollOuterHeight);

            int rowsDrawn = 0;

            // enumeration sections
            for (int sectionIndex = 1; sectionIndex < Sections.Count; sectionIndex++)
            {
                var section = Sections[sectionIndex];
                var sectionYMin = GUIScale(Sections.IntegralSizeOf(sectionIndex) - offsetY);
                var sectionYMax = GUIScale(Sections.IntegralSizeOf(sectionIndex + 1) - offsetY);

                // if section is visible
                if (sectionYMin < maxY &&
                    sectionYMax > minY)
                {
                    var sectionRect = Rect.MinMaxRect(-hScrollPosition, sectionYMin, scrollInnerWidth - hScrollPosition, sectionYMax);

                    section.Section.DoScrollableBackground(sectionRect, visibleRect);

                    // enumerating columns
                    for (int columnIndex = 1; columnIndex < Columns.Count; columnIndex++)
                    {
                        var cellXMin = GUIScale(Columns.IntegralSizeOf(columnIndex) - offsetX);
                        var cellXMax = GUIScale(Columns.IntegralSizeOf(columnIndex + 1) - offsetX);
                        
                        // is column visible
                        if (cellXMin < maxX &&
                            cellXMax > minX)
                        {
                            var column = Columns[columnIndex];
                            var columnRect = Rect.MinMaxRect(cellXMin, sectionYMin, cellXMax, sectionYMax);
                            column.DoBackground(columnRect, visibleRect);

                            // enumerating rows
                            var firstCombined = -1;
                            var highlightedCombined = false;
                            for (int rowIndex = 0; rowIndex < section.Count; rowIndex++)
                            {
                                var row = section[rowIndex];

                                // if can combine rows in this column (mechanitor column for mechs, for example)
                                if (section.Section.CanMergeRows(columnIndex))
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
                                        var cellYMin = sectionYMin + section.IntegralSizeOf(beginIndex);
                                        var cellYMax = sectionYMin + section.IntegralSizeOf(rowIndex + 1);

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
                                    var cellYMin = sectionYMin + section.IntegralSizeOf(rowIndex);
                                    var cellYMax = sectionYMin + section.IntegralSizeOf(rowIndex + 1);

                                    // render cell if visible
                                    if (cellYMin < maxY &&
                                        cellYMax > minY)
                                    {
                                        row.DoCell(
                                            Rect.MinMaxRect(cellXMin, cellYMin, cellXMax, cellYMax),
                                            columnIndex,
                                            highlightedSectionIndex == sectionIndex && highlightedRowIndex == rowIndex,
                                            false);
                                        rowsDrawn++;
                                    }
                                }
                            }

                            column.DoOverlay(columnRect, visibleRect);
                        }
                    }

                    section.Section.DoScrollableOverlay(sectionRect, visibleRect);
                }
            }

            $"rows drawn: {rowsDrawn}".Log();

            GUI.EndGroup();
        }

        private void DoGridOverlays()
        {
            var minY = 0;
            var maxY = scrollOuterHeight;
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
    }

}
