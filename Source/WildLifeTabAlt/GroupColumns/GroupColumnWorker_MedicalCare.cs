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
    class GroupColumnWorker_MedicalCare : GroupColumnWorker
    {
        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => p?.playerSettings?.medCare);
        }

        public override bool CanSetValues()
        {
            return false;
        }

        public override object GetGroupValue(IEnumerable<Pawn> pawns)
        {
            return null;
        }

        public override void SetGroupValue(IEnumerable<Pawn> pawns, object value)
        {
            
        }

        public override object DefaultValue()
        {
            return MedicalCareCategory.HerbalOrWorse; // todo: def value
        }

        public override object GetValue(Pawn pawn)
        {
            return pawn.playerSettings?.medCare;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            if (pawn.playerSettings != null)
            {
                pawn.playerSettings.medCare = (MedicalCareCategory)value;
            }
        }

        public override bool IsVisible(IEnumerable<Pawn> pawns)
        {
            return Mod.Settings.interactiveGroupHeaderExperimental;
        }

    }
}
