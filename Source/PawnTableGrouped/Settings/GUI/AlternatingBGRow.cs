using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class AlternatingBGRow : CListingRow
    {
        public bool IsOdd = false;

        public override void DoContent()
        {
            base.DoContent();
            if (IsOdd)
            {
                Widgets.DrawAltRect(BoundsRounded);
            }
        }
    }
}
