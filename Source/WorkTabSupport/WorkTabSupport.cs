using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.WorkTab
{
    class WorkTabSupport : ModMod
    {
        public WorkTabSupport()
        {
            Mod.RegisterModBridge(ModMod.WorkTabPackageId, WorkTabBridge.Instance);
        }
    }
}
