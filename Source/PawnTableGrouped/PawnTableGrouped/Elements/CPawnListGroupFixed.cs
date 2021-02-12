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
        static readonly Resource<Texture2D> TexCollapse = new Resource<Texture2D>("UI/Buttons/Dev/Collapse");
        static readonly Resource<Texture2D> TexRevial = new Resource<Texture2D>("UI/Buttons/Dev/Reveal");

        private ClVariable rightTitleEdge;

        public CPawnListGroupFixed(PawnTableGroup group, bool expanded)
        {

            var img = this.AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = this.AddElement(new CLabel
            {
                TaggedTitle = group.Title,
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
                groupRow.overflow = (float)rightTitleEdge.Value + Metrics.GroupTitleRightMargin - BoundsRounded.width;
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
