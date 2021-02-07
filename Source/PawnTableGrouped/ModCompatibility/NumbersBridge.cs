using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Numbers;

namespace PawnTableGrouped
{
    public class NumbersBridge : ModBridge<NumbersBridge>
    { 
        public static bool IsNumbersTable(PawnTable table)
        {
            return Instance.IsActive && table != null && tableType.IsAssignableFrom(table.GetType());
        }


        public static int ReorderableGroup(PawnTable pawnTable)
        {
            if (!Instance.IsActive)
            {
                return 0;
            }

            try
            {
                return numbersReorderableGroup(pawnTable);                
            }
            catch
            {
                Instance.Deactivate();
                return 0;
            }
        }

        static Type tableType = null;
        public static void CallReorderableWidget(int groupId, Rect rect)
        {
            if (!Instance.IsActive)
            {
                return;
            }

            try
            {
                numbersCallReorderableWidget(groupId, rect);
            }
            catch
            {
                Instance.Deactivate();
            }
        }

        static Func<PawnTable, int> numbersReorderableGroup;
        static Action<int, Rect> numbersCallReorderableWidget;

        protected override bool ResolveInternal(HarmonyLib.Harmony harmony)
        {
            tableType = typeof(PawnTable_NumbersMain);
            numbersReorderableGroup = FastAccess.CreateStaticRetMethodWrapper<PawnTable, int>(typeof(Numbers.Numbers).GetMethod("ReorderableGroup", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));

            numbersCallReorderableWidget = FastAccess.CreateStaticVoidMethodWrapper<int, Rect>(typeof(Numbers.Numbers).GetMethod("CallReorderableWidget", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));

            return 
                numbersReorderableGroup != null &&
                numbersCallReorderableWidget != null &&
                tableType != null;
        }

        public override string ModName()
        {
            return "Numbers";
        }
    }

}
