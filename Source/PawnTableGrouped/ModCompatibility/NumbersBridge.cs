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
using RWLayout.alpha2.FastAccess;

namespace PawnTableGrouped
{
    public interface INumbersBridge
    {
        public bool IsNumbersTable(PawnTable table);
        public int ReorderableGroup(PawnTable pawnTable);
        public void CallReorderableWidget(int groupId, Rect rect);
    }

    public class NumbersPlaceholderBridge : PlaceholderBridge, INumbersBridge
    {
        public void CallReorderableWidget(int groupId, Rect rect) { }

        public bool IsNumbersTable(PawnTable table) => false;
        public int ReorderableGroup(PawnTable pawnTable) => 0;
    }

    public class NumbersBridge : ModBridge<NumbersBridge>, INumbersBridge
    { 
        public bool IsNumbersTable(PawnTable table)
        {
            return IsActive && table != null && tableType.IsAssignableFrom(table.GetType());
        }

        public int ReorderableGroup(PawnTable pawnTable)
        {
            if (!IsActive)
            {
                return 0;
            }

            try
            {
                return numbersReorderableGroup(pawnTable);                
            }
            catch
            {
                Deactivate();
                return 0;
            }
        }

        static Type tableType = null;
        public void CallReorderableWidget(int groupId, Rect rect)
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
            numbersReorderableGroup = Dynamic.StaticRetMethod<PawnTable, int>(typeof(Numbers.Numbers).GetMethod("ReorderableGroup", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));

            numbersCallReorderableWidget = Dynamic.StaticVoidMethod<int, Rect>(typeof(Numbers.Numbers).GetMethod("CallReorderableWidget", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));

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
