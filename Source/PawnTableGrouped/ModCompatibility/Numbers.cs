using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{

    public class NumbersWrapper
    {
        static bool disabled = true;
        static Type numbersType = null;
        static Type pawnTableType = null;

        public static bool IsActive
        {
            get
            {
                return !disabled;
            }
        }

        public static bool IsNumbersTable(PawnTable table)
        {
            return IsActive && NumbersTableType.IsAssignableFrom(table.GetType());
        }

        public static Type NumbersTableType
        {
            get
            {
                if (disabled)
                {
                    return null;
                }

                return pawnTableType;
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
                return (int)numbersType.GetMethod("ReorderableGroup", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { pawnTable });
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
                numbersType.GetMethod("CallReorderableWidget", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { groupId, rect });
            }
            catch
            {
                disabled = true;
            }
        }

        public static void Resolve()
        {
            if (!Mod.ModNumbersActive)
            {
                disabled = true;
            }

            try
            {
                numbersType = GenTypes.GetTypeInAnyAssembly("Numbers.Numbers");
                pawnTableType = GenTypes.GetTypeInAnyAssembly("Numbers.PawnTable_NumbersMain");
                disabled = false;
            }
            catch
            {
                disabled = true;
            }
        }
    }

}
