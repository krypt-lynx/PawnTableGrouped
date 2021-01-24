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

    public class ColonyGroupsBridge : ModBridge<ColonyGroupsBridge>
    {
#if rw_1_2
        static PropertyInfo AllPawnGroupsProp = null;

        public static List<T> AllPawnGroups<T>()
        {
            if (!Instance.IsActive)
            {
                return null;
            }

            return (List<T>)AllPawnGroupsProp.GetValue(null);
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
#else
        protected override bool ResolveInternal(Harmony harmony)
        {
            return false;
        }
#endif

        public override string ModName()
        {
            return "Colony Groups";
        }
    }


}
