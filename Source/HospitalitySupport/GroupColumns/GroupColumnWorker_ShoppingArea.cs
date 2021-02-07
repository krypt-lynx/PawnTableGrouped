using Hospitality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped.Hospitality
{
    class GroupColumnWorker_ShoppingArea : GroupColumnWorker_Area
    {
        private static Func<Pawn, CompGuest> call_Pawn_CompGuest = FastAccess.StaticRetMethod<Pawn, CompGuest>(GenTypes.GetTypeInAnyAssembly("Hospitality.CompUtility").GetMethod("CompGuest", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

        public override string GetAreaLabel(Area area)
        {
            if (area != null)
            {
                return AreaUtility.AreaAllowedLabel_Area(area);
            }
            return "AreaNoShopping".Translate();
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return pawns.FirstOrDefault()?.Map?.GetComponent<Hospitality_MapComponent>()?.defaultAreaShopping;
        }

        public override object GetValue(Pawn pawn)
        {
            CompGuest compGuest = call_Pawn_CompGuest(pawn);
            if (compGuest == null)
            {
                return null;
            }
            return compGuest.ShoppingArea;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            CompGuest compGuest = call_Pawn_CompGuest(pawn);
            if (compGuest != null)
            {
                compGuest.ShoppingArea = (Area)value;
            }
        }
    }
}
