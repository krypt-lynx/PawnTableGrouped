using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt.GroupColumns
{
    class GroupColumnWorker_MedicalCare : GroupColumnResolver
    {
        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => p?.playerSettings?.medCare);
        }
    }
}
