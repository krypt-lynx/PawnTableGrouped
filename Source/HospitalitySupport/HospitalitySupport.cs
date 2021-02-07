using PawnTableGrouped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGroupsSupport
{
    public class HospitalitySupport : ModMod
    {
        public HospitalitySupport()
        {
            Mod.RegisterModBridge("orion.hospitality", HospitalityBridge.Instance);
            //Mod.RegisterGroupWorker(new GroupWorker_ByColonyGroup());
        }
    }
}
