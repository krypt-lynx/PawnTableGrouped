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
    public class SimpleSlaveryBridge : ModBridge<SimpleSlaveryBridge>
    {
        public static bool IsPawnColonySlave(Pawn pawn)
        {
            if (!Instance.IsActive)
            {
                return false;
            }
            return SlaveUtility.IsPawnColonySlave(pawn);
        }

        protected override bool ResolveInternal()
        {
            return ((Func<Pawn, bool>)SlaveUtility.IsPawnColonySlave)?.Method != null; // ensure method exists
        }
    }

}
