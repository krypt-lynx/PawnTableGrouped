using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PawnTableGrouped.TableGrid
{
    public interface ICTableGridColumn
    {
        public void DoBackground(Rect rect, Rect visibleRect);
        public void DoOverlay(Rect rect, Rect visibleRect);
    }
}
