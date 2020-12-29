using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt
{


    class CPawnListSection : CListingRow
    {
        static readonly Texture2D TexCollapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);
        static readonly Texture2D TexRevial = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

        private PawnTable table;
        private PawnTableAccessor accessor;
        public PawnTableGroup Group;

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(0, 30);
        }

        public Action<CElement> Action { get; set; }
        public CPawnListSection(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group, bool expanded)
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

            this.StackLeft(StackOptions.Create(constrainSides: false), img, label);
            img.MakeSizeIntristic();

            this.AddConstraints(
                img.centerY ^ centerY,
                label.centerY ^ centerY,
                label.height ^ label.intrinsicHeight
                );

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
                var resolver = Group.ColumnResolvers[columnIndex];
                if (resolver != null && resolver.IsVisible(Group.Pawns))
                {
                    Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, (int)BoundsRounded.height);
                    resolver.DoCell(cellRect, Group, table, columnIndex);
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


    /*

        public override void DoContent()
        {
            base.DoContent();

            int x = (int)BoundsRounded.xMin;
            var columns = table.ColumnsListForReading;


            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.yMin, BoundsRounded.width);
            GUI.color = Color.white;
            if (!accessor.CanAssignPawn(pawn))
            {
                GUI.color = Color.gray;
            }

            if (Mouse.IsOver(BoundsRounded))
            {
                GUI.DrawTexture(BoundsRounded, TexUI.HighlightTex);
                target.Highlight(true, pawn.IsColonist, false);
            }
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
                Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, (int)BoundsRounded.height);
                columns[columnIndex].Worker.DoCell(cellRect, pawn, table);
                x += columnWidth;
            }
            if (pawn.Downed)
            {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.center.y, BoundsRounded.width);
            }
            GUI.color = Color.white;
        }    
    */
}
