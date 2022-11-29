using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawnTableGrouped.TableGrid;
using RWLayout.alpha2;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    class CPawnTableColumn : ICTableGridColumn
    {
        private readonly bool highlight;

        public CPawnTableColumn(bool highlight)
        {
            this.highlight = highlight;
        }

        public void DoBackground(Rect rect, Rect visibleRect)
        {
            if (highlight)
            {
                GUI.DrawTexture(rect, Textures.AltTexture);
            }    
        }

        public void DoOverlay(Rect rect, Rect visibleRect) { }
    }
}
