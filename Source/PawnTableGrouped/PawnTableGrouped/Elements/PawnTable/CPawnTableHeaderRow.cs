using PawnTableGrouped.TableGrid;
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
    class CPawnTableHeaderRow : ICTableGridRow
    {
        private PawnTableGroupedModel model;

        public CPawnTableHeaderRow(PawnTableGroupedModel model)
        {
            this.model = model;
        }

        public bool CanCombineWith(ICTableGridRow other, int columnIndex)
        {
            return false;
        }

        public void DoBackground(Rect rect) { }

        public void DoCell(Rect rect, int columnIndex, bool hightlighted, bool combined)
        {
            if (columnIndex == 0)
            {
                rect = rect.ContractedBy(new EdgeInsets(0, 0, 0, Metrics.TableLeftMargin));
            }

            if (KnownMods.Numbers.IsNumbersTable(model.Table))
            {
                KnownMods.Numbers.CallReorderableWidget(model.NumbersMagic, rect);
            }

            model.Table.Columns()[columnIndex].Worker.DoHeader(rect, model.Table);
        }

        public void DoOverlay(Rect rect) { }

        public float GetRowHeight()
        {
            return model.Table.GetCachedHeaderHeight();

        }
    }    
}
