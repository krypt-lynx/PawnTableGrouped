using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
#if rw_1_2
using SimpleSlavery;
#endif

namespace PawnTableGrouped
{
    public class SimpleSlaveryBridge : ModBridge<SimpleSlaveryBridge>
    {
        public static bool IsPawnColonySlave(Pawn pawn)
        {
#if rw_1_2
            if (!Instance.IsActive)
            {
                return false;
            }
            return SlaveUtility.IsPawnColonySlave(pawn);
#else 
            return false;
#endif

        }

        protected override bool ResolveInternal(HarmonyLib.Harmony harmony)
        {
#if rw_1_2
            return ((Func<Pawn, bool>)SlaveUtility.IsPawnColonySlave)?.Method != null; // ensure method exists
#else 
            return false;
#endif
        }
    }
}