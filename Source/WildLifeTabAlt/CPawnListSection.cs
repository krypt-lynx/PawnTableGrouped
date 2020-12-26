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
        private List<GroupColumnResolver> columnResolvers;
        public PawnTableGroup Group;

        public Action<CElement> Action { get; set; }
        public CPawnListSection(PawnTable table, PawnTableAccessor accessor, PawnTableGroup group, List<GroupColumnResolver> columnResolvers, bool expanded)
        {
            this.table = table;
            this.accessor = accessor;
            this.Group = group;
            this.columnResolvers = columnResolvers;

            var img = AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = AddElement(new CLabel
            {
                Title = $"{group.Title ?? "<Unknown>"} ({group.Pawns?.Count ?? 0})",
                Font = GameFont.Small,
                Color = Color.grey,
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

            if (Widgets.ButtonInvisible(BoundsRounded))
            {                
                this.Action?.Invoke(this);
            }

            if (Event.current.type == EventType.Repaint)
            {
                DoRowsSummary();
            }
        }

        private void DoRowsSummary()
        {
            int x = (int)BoundsRounded.xMin;
            var columns = table.ColumnsListForReading;


            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(BoundsRounded.xMin, BoundsRounded.yMin, BoundsRounded.width);
            GUI.color = Color.white;
            //if (!accessor.CanAssignPawn(pawn))
            //{
            //    GUI.color = Color.gray;
            //}

            //if (Mouse.IsOver(BoundsRounded))
            //{
            //    GUI.DrawTexture(BoundsRounded, TexUI.HighlightTex);
            //    target.Highlight(true, pawn.IsColonist, false);
            //}
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
                var resolver = columnResolvers[columnIndex];
                if (resolver != null)
                {
                    Rect cellRect = new Rect(x, BoundsRounded.yMin, columnWidth, (int)BoundsRounded.height);
                    resolver.DoCell(cellRect, Group, table);
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
