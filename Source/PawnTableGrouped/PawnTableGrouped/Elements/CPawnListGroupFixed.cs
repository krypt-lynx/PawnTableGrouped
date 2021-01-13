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
    class CPawnListGroupRow : CPawnTableRow
    {
        public Action<CPawnListGroupRow> Action { get; set; }
        public PawnTableGroup Group;
        public float overflow;
    }

    class CPawnListGroupFixed : CRowSegment
    {
        static readonly Texture2D TexCollapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);
        static readonly Texture2D TexRevial = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

        private ClVariable rightTitleEdge;

        public CPawnListGroupFixed(PawnTableGroup group, bool expanded)
        {

            var img = this.AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = this.AddElement(new CLabel
            {
                TaggedTitle = group.Title + $" ({group.Pawns?.Count ?? 0})",
                Font = GameFont.Small,
                Color = new Color(1, 1, 1, 0.6f),
                TextAlignment = TextAnchor.MiddleLeft,
            });

            this.StackLeft(StackOptions.Create(constrainSides: false, constrainEnd: false), img, (label, label.intrinsicWidth));
            img.MakeSizeIntristic();

            this.AddConstraints(
                img.centerY ^ this.centerY,
                label.centerY ^ this.centerY,
                label.height ^ label.intrinsicHeight
                );

            rightTitleEdge = label.right;
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(0, Metrics.GroupHeaderHeight);
        }

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            if (Row is CPawnListGroupRow groupRow)
            {
                groupRow.overflow = (float)rightTitleEdge.Value - BoundsRounded.width;
            }
        }

        public override void DoContent()
        {
            base.DoContent();

            if (Widgets.ButtonInvisible(BoundsRounded))
            {
                (Row as CPawnListGroupRow)?.Action?.Invoke((CPawnListGroupRow)Row);
            }
        }
    }
}
