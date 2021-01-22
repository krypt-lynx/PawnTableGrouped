using HarmonyLib;
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

#if rw_1_2
   public class ColonyGroupsBridge : ModBridge<ColonyGroupsBridge>
    {
        static PropertyInfo AllPawnGroupsProp = null;

        public static List<PawnGroup> AllPawnGroups
        {
            get
            {
                if (!Instance.IsActive)
                {
                    return new List<PawnGroup>();
                }

                return (List<PawnGroup>)AllPawnGroupsProp.GetValue(null);
            }
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
            var tacticUtilsType = GenTypes.GetTypeInAnyAssembly("TacticalGroups.TacticUtils");
            AllPawnGroupsProp = tacticUtilsType.GetProperty("AllPawnGroups", BindingFlags.Public | BindingFlags.Static);

            return AllPawnGroupsProp != null;
        }
    }
#else
    public class ColonyGroupsBridge : ModBridge<ColonyGroupsBridge>
    {
        protected override bool ResolveInternal(Harmony harmony)
        {
            return false;
        }
    }


#endif
}
