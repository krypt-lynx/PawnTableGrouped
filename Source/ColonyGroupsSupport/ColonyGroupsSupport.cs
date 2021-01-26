using PawnTableGrouped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGroupsSupport
{
    public class ColonyGroupsSupport : ModMod
    {
        public ColonyGroupsSupport()
        {
            Mod.RegisterModBridge("derekbickley.ltocolonygroupsfinal", ColonyGroupsBridge.Instance);
            Mod.RegisterGroupWorker(new GroupWorker_ByColonyGroup());
        }
    }
}
