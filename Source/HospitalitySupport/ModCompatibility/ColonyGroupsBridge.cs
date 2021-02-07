using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Hospitality;

namespace PawnTableGrouped
{

    public class HospitalityBridge : ModBridge<HospitalityBridge>
    {
        protected override void ApplyPatches(Harmony harmony)
        {
     
        }

        static void MarkColonistsDirty_postfix()
        {
            EventBus<PawnTableInvalidateMessage>.SendMessage(null, new PawnTableInvalidateMessage());
        }

        protected override bool ResolveInternal(Harmony harmony)
        {
            // test basic compatibility
            return GenTypes.GetTypeInAnyAssembly("Hospitality.CompUtility").GetMethod("CompGuest", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) != null;
        }

        public override string ModName()
        {
            return "Hospitality";
        }
    }
}
