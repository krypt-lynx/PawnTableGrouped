using RimWorld;
using SimpleSlavery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class SimpleSlaveryBridge
    {
        static bool disabled = true;

        public static bool IsActive
        {
            get
            {
                return !disabled;
            }
        }

        public static bool IsPawnColonySlave(Pawn pawn)
        {
            if (!IsActive)
            {
                return false;
            }
            return SlaveUtility.IsPawnColonySlave(pawn);
        }


        public static void Resolve(bool active)
        {
            if (!active)
            {
                disabled = true;
                return;
            }

            try
            {
                disabled = GenTypes.GetTypeInAnyAssembly("SimpleSlavery.SlaveUtility") == null || 
                    typeof(SlaveUtility).GetMethod("IsPawnColonySlave", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Pawn) }, null) == null;
            }
            catch
            {
                disabled = true;
            }
        }
    }

}
