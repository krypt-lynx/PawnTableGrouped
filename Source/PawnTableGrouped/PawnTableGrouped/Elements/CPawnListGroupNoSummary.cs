using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class CPawnListGroupNoSummary : CRowSegment
    {
        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(0, Metrics.GroupHeaderHeight);
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
