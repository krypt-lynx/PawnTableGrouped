﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

#if rw_1_2
using TacticalGroups;
#endif
namespace PawnTableGrouped
{

    public class ColonyGroupsBridge : ModBridge<ColonyGroupsBridge>
    {
#if rw_1_2
       public static List<PawnGroup> AllPawnGroups()
        {
            if (!Instance.IsActive)
            {
                return null;
            }

            return TacticUtils.AllPawnGroups;
        }

        protected override void ApplyPatches(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(TacticalColonistBar), "MarkColonistsDirty"),
                postfix: new HarmonyMethod(typeof(ColonyGroupsBridge), "MarkColonistsDirty_postfix")
                );
        }

        static void MarkColonistsDirty_postfix()
        {
            EventBus<PawnTableInvalidateMessage>.SendMessage(null, new PawnTableInvalidateMessage());
        }

        protected override bool ResolveInternal(Harmony harmony)
        {
            // Enure property exists and have right type.
            return typeof(IEnumerable<PawnGroup>).IsAssignableFrom(AccessTools.Property(typeof(TacticUtils), nameof(TacticUtils.AllPawnGroups)).ReflectedType);
        }
#else
        protected override bool ResolveInternal(Harmony harmony)
        {
            $"rw_1_2 is not defined".Log(MessageType.Error);
            
            return false;
        }
#endif

        public override string ModName()
        {
            return "Colony Groups";
        }
    }


}
