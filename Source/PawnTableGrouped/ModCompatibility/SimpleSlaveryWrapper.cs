using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class SimpleSlaveryWrapper
    {
        static bool disabled = true;
        static Type simpleSlaveryType = null;

        static MethodInfo isPawnColonySlaveMethod = null;

        public static bool IsActive
        {
            get
            {
                return !disabled;
            }
        }

        public static bool IsPawnColonySlave(Pawn pawn)
        {
            if (!IsActive)
            {
                return false;
            }            
            return (bool)isPawnColonySlaveMethod?.Invoke(null, new object[] { pawn });
        }

        public static Type SimpleSlaveryType
        {
            get
            {
                if (disabled)
                {
                    return null;
                }

                return simpleSlaveryType;
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
                simpleSlaveryType = GenTypes.GetTypeInAnyAssembly("SimpleSlavery.SlaveUtility");
                isPawnColonySlaveMethod = simpleSlaveryType.GetMethod("IsPawnColonySlave", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Pawn) }, null);


                disabled = false;
            }
            catch
            {
                disabled = true;
            }
        }
    }

}
