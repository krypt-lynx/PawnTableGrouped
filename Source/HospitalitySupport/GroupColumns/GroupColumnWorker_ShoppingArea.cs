using Hospitality;
using RimWorld;
using RWLayout.alpha2.FastAccess;
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
        private static Func<Pawn, CompGuest> call_Pawn_CompGuest = Dynamic.StaticRetMethod<Pawn, CompGuest>(GenTypes.GetTypeInAnyAssembly("Hospitality.CompUtility").GetMethod("CompGuest", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

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
            return new AreaData(pawns.FirstOrDefault()?.Map?.GetComponent<Hospitality_MapComponent>()?.defaultAreaShopping);
        }

        public override object GetValue(Pawn pawn)
        {
            CompGuest compGuest = call_Pawn_CompGuest(pawn);
            if (compGuest == null)
            {
                return AreaData.Undefined;
            }
            return new AreaData(compGuest.ShoppingArea);
        }

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            CompGuest compGuest = call_Pawn_CompGuest(pawn);
            if (compGuest != null)
            {
                var data = (AreaData)value;
                if (data.Type == AreaData.DataType.Area)
                {
                    compGuest.ShoppingArea = data.Area;
                }
            }
        }
    }
}
