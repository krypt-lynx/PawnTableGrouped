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
    class CTableGridHeaderSection : ICTableGridSection
    {
        private readonly PawnTableGroupedModel model;

        public CTableGridHeaderSection(PawnTableGroupedModel model)
        {
            this.model = model;
        }

        public ICTableGridRow RowAt(int row) => new CPawnTableHeaderRow(model);

        public bool CanMergeRows(int column) => false;

        public int NumberOfRows() => 1;

        public void DoScrollableBackground(Rect rect, Rect visibleRect)
        {

        }

        public void DoScrollableOverlay(Rect rect, Rect visibleRect)
        {
            const float easeInRange = 64;
            const float maxShadowWidth = 16;
            var fadeIn = Mathf.Clamp(visibleRect.xMin - rect.xMin, 0, easeInRange) / easeInRange;
            var shadowWidth = maxShadowWidth * fadeIn;
            var shadowColor = new Color(14 / 255f, 16 / 255f, 20 / 255f, fadeIn);
            GuiTools.PushColor(shadowColor);
            var shadowRect = Rect.MinMaxRect(visibleRect.xMin, rect.yMin /* + Metrics.GroupHeaderHeight */, visibleRect.xMin + shadowWidth, rect.yMax);

            GUI.DrawTexture(shadowRect, Textures.RadialGradient);
            GuiTools.PopColor();
        }
    }
}
