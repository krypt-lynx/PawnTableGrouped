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
    class CTableGridGroupSection : ICTableGridSection
    {
        private readonly PawnTableGroupedModel model;
        private readonly PawnTableGroup group;
        private readonly float fixedWidth;

        public CTableGridGroupSection(PawnTableGroupedModel model, PawnTableGroup group, float fixedWidth)
        {
            this.model = model;
            this.group = group;
            this.fixedWidth = fixedWidth;
        }

        public bool CanMergeRows(int column)
        {
#if rw_1_4_or_later
            return model.Table.Columns[column].groupable;
#else
            return false;
#endif
        }

        public void DoScrollableBackground(Rect rect, Rect visibleRect)
        {

        }
        //static Color shadowColor = new ColorInt(14, 16, 20).ToColor;
        public void DoScrollableOverlay(Rect rect, Rect visibleRect)
        {
            const float easeInRange = 64;
            const float maxShadowWidth = 16;
            var fadeIn = Mathf.Clamp(visibleRect.xMin - rect.xMin, 0, easeInRange) / easeInRange;
            if (fadeIn > 0.001)
            {
                var shadowWidth = maxShadowWidth * fadeIn;
                var shadowColor = new Color(14 / 255f, 16 / 255f, 20 / 255f, fadeIn);
                GuiTools.PushColor(shadowColor);
                var shadowRect = Rect.MinMaxRect(visibleRect.xMin, rect.yMin /* + Metrics.GroupHeaderHeight */, visibleRect.xMin + shadowWidth, rect.yMax);

                GUI.DrawTexture(shadowRect, Textures.LinearGradient);
                GuiTools.PopColor();
            }
        }

        public int NumberOfRows()
        {
            bool showHeader = !Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1;

            if (model.IsExpanded(group))
            {
                return group.Pawns.Count + (showHeader ? 1 : 0);
            }
            else
            {
                return showHeader ? 1 : 0;
            }
        }

        public ICTableGridRow RowAt(int row)
        {
            //var weakThis = new Verse.WeakReference<PawnTableGroupedView>(this);
            bool showHeader = !Mod.Settings.hideHeaderIfOnlyOneGroup || model.Groups.Count > 1;

            if (showHeader && row == 0)
            {
                var groupHeader = new CPawnTableGroupRow(group, fixedWidth, model.IsExpanded(group));
                groupHeader.Action = (group) =>
                {
                    model.SwitchExpanded(group);
                };
                return groupHeader;
            }
            else
            {
                var pawn = group.Pawns[row - (showHeader ? 1 : 0)];
                return new CPawnTablePawnRow(model.Table, group, pawn);
            }
        }
    }
}
