using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PawnTableGrouped.TableGrid
{
    public interface ICTableGridSection
    {
        int NumberOfRows();
        ICTableGridRow RowAt(int row);
        bool CanMergeRows(int column);
        void DoScrollableBackground(Rect rect, Rect visibleRect);
        void DoScrollableOverlay(Rect rect, Rect visibleRect);
    }
}
