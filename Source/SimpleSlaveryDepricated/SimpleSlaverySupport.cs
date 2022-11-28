using PawnTableGrouped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGroupsSupport
{
    public class SimpleSlaverySupport : ModMod
    {
        public SimpleSlaverySupport()
        {
            Mod.RegisterModBridge("syl.simpleslavery", SimpleSlaveryBridge.Instance);
            Mod.RegisterGroupWorker(new GroupWorker_IsSlave());
        }
    }
}
