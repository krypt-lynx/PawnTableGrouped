﻿using RimWorld;
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
                return (int)typeof(Numbers.Numbers).GetMethod("ReorderableGroup", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { pawnTable });
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
                typeof(Numbers.Numbers).GetMethod("CallReorderableWidget", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { groupId, rect });
            }
            catch
            {
                Instance.Deactivate();
            }
        }

        protected override bool ResolveInternal()
        {
            tableType = typeof(PawnTable_NumbersMain);
            return GenTypes.GetTypeInAnyAssembly("Numbers.Numbers") != null &&
                GenTypes.GetTypeInAnyAssembly("Numbers.PawnTable_NumbersMain") != null;
        }
    }

}
