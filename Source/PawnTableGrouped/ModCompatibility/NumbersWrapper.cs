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
    public class NumbersWrapper
    {
        static bool disabled = true;
        //static Type numbersType = null;
        //static Type pawnTableType = null;

        public static bool IsActive
        {
            get
            {
                return !disabled;
            }
        }

        public static bool IsNumbersTable(PawnTable table)
        {
            return IsActive && table != null && NumbersTableType.IsAssignableFrom(table.GetType());
        }

        public static Type NumbersTableType
        {
            get
            {
                if (disabled)
                {
                    return null;
                }

                return typeof(PawnTable_NumbersMain);
            }
        }

        public static int ReorderableGroup(PawnTable pawnTable)
        {
            if (disabled)
            {
                return 0;
            }

            try
            {
                return (int)typeof(Numbers.Numbers).GetMethod("ReorderableGroup", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { pawnTable });
            }
            catch
            {
                disabled = true;
                return 0;
            }
        }

        public static void CallReorderableWidget(int groupId, Rect rect)
        {
            if (disabled)
            {
                return;
            }

            try
            {
                typeof(Numbers.Numbers).GetMethod("CallReorderableWidget", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { groupId, rect });
            }
            catch
            {
                disabled = true;
            }
        }

        public static void Resolve(bool active)
        {
            if (!active)
            {
                disabled = true;
                return;
            }

            try
            {
                disabled = GenTypes.GetTypeInAnyAssembly("Numbers.Numbers") == null ||
                           GenTypes.GetTypeInAnyAssembly("Numbers.PawnTable_NumbersMain") == null;
            }
            catch
            {
                disabled = true;
            }
        }
    }

}
