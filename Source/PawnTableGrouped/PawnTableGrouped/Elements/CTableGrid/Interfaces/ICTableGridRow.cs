using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PawnTableGrouped.TableGrid
{
    public interface ICTableGridRow
    {
        public void DoCell(Rect rect, int columnIndex, bool hightlighted, bool combined);
        public void DoBackground(Rect rect);
        public void DoOverlay(Rect rect);
        public bool CanCombineWith(ICTableGridRow other, int columnIndex);
        public float GetRowHeight();
    }
}
