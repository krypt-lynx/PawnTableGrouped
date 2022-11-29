using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public static class KnownMods
    {
        public static Dictionary<string, IModBridge> DetectedBridges = new Dictionary<string, IModBridge>();

        // mods I interacting with directly
        public static IWorkTabBridge WorkTab;
        public static INumbersBridge Numbers;

        internal static void AddModBridge(string packageId, ModBridge bridge)
        {
            KnownMods.DetectedBridges[packageId] = bridge;
            switch (packageId)
            {
                case ModMod.WorkTabPackageId:
                    WorkTab = bridge as IWorkTabBridge;
                    break;
                case ModMod.NumbersPackageId:
                    Numbers = bridge as INumbersBridge;
                    break;
            }
        }

        internal static void FinalizeInit()
        {
            WorkTab ??= new WorkTabPlaceholderBridge();
            Numbers ??= new NumbersPlaceholderBridge();
        }
    }

}
