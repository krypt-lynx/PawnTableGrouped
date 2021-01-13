using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PawnTableGrouped
{
    class PawnListHeader : CElement
    {
        private PawnTableGroupedModel model;
        Func<float> getScrollOffset;
        public PawnListHeader(PawnTableGroupedModel model, Func<float> getScrollOffset)
        {
            this.model = model;
            this.getScrollOffset = getScrollOffset;
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


            var column0Width = columns.Count > 0 ? cachedColumnWidths[0] : 0;

            float width = BoundsRounded.width - 16f - Metrics.TableLeftMargin;
            float xLeft = BoundsRounded.xMin + Metrics.TableLeftMargin;
            float xRight = xLeft + this.Bounds.width; 


            if (columns.Count > 0)
            {
                Rect rect = new Rect(xLeft, BoundsRounded.yMin, column0Width, BoundsRounded.height);
                NumbersWrapper.CallReorderableWidget(model.NumbersMagic, rect);
                columns[0].Worker.DoHeader(rect, model.Table);
            }

            
            GUI.BeginClip(Rect.MinMaxRect(xLeft + column0Width, BoundsRounded.yMin, BoundsRounded.xMax, BoundsRounded.yMax));

            float x = (int)(-getScrollOffset());

            for (int columnIndex = 1; columnIndex < columns.Count; columnIndex++)
            {
                int columnWidth;
                if (columnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(width - x); // <-- this is invalid value. // todo: fix invalid last header height
                }
                else
                {
                    columnWidth = (int)model.accessor.cachedColumnWidths[columnIndex];
                }

                if (x + columnWidth > 0 && x <= this.Bounds.width - column0Width)
                {
                    Rect rect = new Rect(x, 0, columnWidth, BoundsRounded.height);
                    NumbersWrapper.CallReorderableWidget(model.NumbersMagic, rect);
                    columns[columnIndex].Worker.DoHeader(rect, model.Table);
                }

                x += columnWidth;
            }

            GUI.EndClip();
        }

    }

}
