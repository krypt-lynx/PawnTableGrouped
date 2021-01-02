using Cassowary;
using RimWorld;
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


    class CPawnListGroup : CListingRow
    {
        static readonly Texture2D TexCollapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);
        static readonly Texture2D TexRevial = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

        private PawnTable table;
        private PawnTableAccessor accessor;
        public PawnTableGroup Group;

        private ClVariable rightTitleEdge;
        
        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(0, Metrics.GroupHeaderHeight);
        }

        public Action<CElement> Action { get; set; }
        public CPawnListGroup(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group, bool expanded)
        {
            this.table = table;
            this.accessor = accessor;
            this.Group = group;

            var img = AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = AddElement(new CLabel
            {
                Title = $"{group.Title ?? "<Unknown>"} ({group.Pawns?.Count ?? 0})",
                Font = GameFont.Small,
                Color = new Color(1, 1, 1, 0.6f),
                TextAlignment = TextAnchor.MiddleLeft,
            });

            this.StackLeft(StackOptions.Create(constrainSides: false, constrainEnd: false), img, (label, label.intrinsicWidth));
            img.MakeSizeIntristic();

            this.AddConstraints(
                img.centerY ^ centerY,
                label.centerY ^ centerY,
                label.height ^ label.intrinsicHeight
                );

            rightTitleEdge = label.right;
        }

        public override void DoContent()
        {
            //  Texture2D tex = node.IsOpen(openMask) ? TexButton.Collapse : TexButton.Reveal;
            base.DoContent();

            DoRowsSummary();

            if (Widgets.ButtonInvisible(BoundsRounded))
            {                
                this.Action?.Invoke(this);
            }

        }

        private void DoRowsSummary()
        {
            int x = (int)(BoundsRounded.xMin + Metrics.TableLeftMargin);
            var columns = table.ColumnsListForReading;


            for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
            {
                int columnWidth;
                if (columnIndex == columns.Count - 1)
                {
                    columnWidth = (int)(BoundsRounded.width - x);
                }
                else
                {
                    columnWidth = (int)accessor.cachedColumnWidths[columnIndex];
                }

                if (x >= rightTitleEdge.Value + Metrics.GroupTitleRightMargin) // hiding cells behind group title
                {
                    var resolver = Group.ColumnResolvers[columnIndex];
                    if (resolver != null && resolver.IsVisible(Group.Pawns))
                    {
                        Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, (int)BoundsRounded.height);
                        resolver.DoCell(cellRect, Group, table, columnIndex);
                    }
                }
                x += columnWidth;
            }
            /*if (pawn.Downed)
            {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.center.y, BoundsRounded.width);
            }*/
            GUI.color = Color.white;
        }
    }   
}
