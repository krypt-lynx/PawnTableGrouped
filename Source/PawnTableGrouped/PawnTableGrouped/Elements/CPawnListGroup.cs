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

    class FloatElement : CElement
    {
        public float xScrollOffset = 0;
        public float visibleRectWidth = 0;

        public override void DoElementContent()
        {
            // offseting render location to make it float
            GUI.BeginGroup(new Rect(xScrollOffset, 0, BoundsRounded.xMax, BoundsRounded.yMax)); // offsetng UI render
            base.DoElementContent();
            GUI.EndGroup();
        }
    }

    class CPawnListGroup : CRowSegment
    {
        static readonly Texture2D TexCollapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);
        static readonly Texture2D TexRevial = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

        private PawnTable table;
        private PawnTableAccessor accessor;
        public PawnTableGroup Group;
        private FloatElement floating;
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

            floating = AddElement(new FloatElement());
            this.Embed(floating);
            var img = floating.AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = floating.AddElement(new CLabel
            {
                TaggedTitle = (group.Title) + $" ({group.Pawns?.Count ?? 0})",
                Font = GameFont.Small,
                Color = new Color(1, 1, 1, 0.6f),
                TextAlignment = TextAnchor.MiddleLeft,
            });

            this.StackLeft(StackOptions.Create(constrainSides: false, constrainEnd: false), img, (label, label.intrinsicWidth));
            img.MakeSizeIntristic();

            this.AddConstraints(
                img.centerY ^ floating.centerY,
                label.centerY ^ floating.centerY,
                label.height ^ label.intrinsicHeight
                );

            rightTitleEdge = label.right;
        }

        public override void DoContent()
        {
            base.DoContent();

            DoRowsSummary();

            if (Widgets.ButtonInvisible(BoundsRounded))
            {                
                this.Action?.Invoke(this);
            }
        }        

        private void DoRowsSummary()
        {
            int x = (int)BoundsRounded.xMin;
            var columns = table.ColumnsListForReading;


            for (int columnIndex = 1; columnIndex < columns.Count; columnIndex++)
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

                if (x + columnWidth > xScrollOffset && x <= xScrollOffset + visibleRectWidth)
                {
                    var resolver = Group.ColumnResolvers[columnIndex];
                    if (resolver != null && Group.IsVisible(columnIndex))
                    {
                        Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, BoundsRounded.height);
                        resolver.DoCell(cellRect, Group.Columns[columnIndex], table);
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
