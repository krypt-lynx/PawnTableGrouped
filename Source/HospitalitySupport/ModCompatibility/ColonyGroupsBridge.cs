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
            return true;
        }

        public override string ModName()
        {
            return "Hospitality";
        }
    }
}
