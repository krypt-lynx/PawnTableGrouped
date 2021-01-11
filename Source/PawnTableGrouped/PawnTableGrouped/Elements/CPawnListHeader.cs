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
    class CPawnListHeader : CElement
    {
        private PawnTableGroupedModel model;

        public CPawnListHeader(PawnTableGroupedModel model)
        {
            this.model = model;
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(model.accessor.cachedHeightNoScrollbar, model.accessor.cachedHeaderHeight);
        }

        public override void DoContent()
        {
            base.DoContent();
            var columns = model.Table.ColumnsListForReading;


            float width = BoundsRounded.width - 16f - Metrics.TableLeftMargin;
            int x = (int)(BoundsRounded.xMin + Metrics.TableLeftMargin);
            for (int headerColumnIndex = 0; headerColumnIndex < columns.Count; headerColumnIndex++)
            {
                int columnWidth;
                if (headerColumnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(width - x);
                }
                else
                {
                    columnWidth = (int)model.accessor.cachedColumnWidths[headerColumnIndex];
                }
                Rect rect = new Rect(BoundsRounded.xMin + x, BoundsRounded.yMin, columnWidth, BoundsRounded.height);

                NumbersWrapper.CallReorderableWidget(model.NumbersMagic, rect);
                columns[headerColumnIndex].Worker.DoHeader(rect, model.Table);
                x += columnWidth;
            }
        }

    }

}
