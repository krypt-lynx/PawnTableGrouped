using Hospitality;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped.Hospitality
{
    class GroupColumnWorker_AccommodationArea : GroupColumnWorker_Area
    {
        private static Func<Pawn, CompGuest> call_Pawn_CompGuest = FastAccess.CreateStaticRetMethodWrapper<Pawn, CompGuest>(GenTypes.GetTypeInAnyAssembly("Hospitality.CompUtility").GetMethod("CompGuest", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return pawns.FirstOrDefault()?.Map?.GetComponent<Hospitality_MapComponent>()?.defaultAreaRestriction;
        }

        public override object GetValue(Pawn pawn)
        {
            CompGuest compGuest = call_Pawn_CompGuest(pawn);
            if (compGuest == null)
            {
                return null;
            }
            return compGuest.GuestArea;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            CompGuest compGuest = call_Pawn_CompGuest(pawn);
            if (compGuest != null)
            {
                compGuest.GuestArea = (Area)value;
            }
        }
    }
}
