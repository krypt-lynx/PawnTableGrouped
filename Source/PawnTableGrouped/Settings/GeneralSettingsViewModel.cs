using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    class GeneralSettingsViewModel
    {
        public bool FixWorkTab {
            get => KnownMods.WorkTab.IsActive && Mod.Settings.fixWorkTab;
            set {
                Mod.Settings.fixWorkTab = value;
                KnownMods.WorkTab.ForcePatchWorkTab = value;
            }
        }

        public bool FixWorkTabDisabled => !KnownMods.WorkTab.IsActive;
    }
}
