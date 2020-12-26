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

        public PawnTableGroup Group;

        public Action<CElement> Action { get; set; }
        public CPawnListSection(PawnTableGroup group, bool expanded)
        {
            this.Group = group;

            var img = AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = AddElement(new CLabel
            {
                Title = $"{group.Title ?? "<Unknown>"} ({group.Pawns?.Count ?? 0})",
                Font = GameFont.Small,
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
        }
    }
}
