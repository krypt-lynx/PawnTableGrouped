using RimWorld;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    static class MainTabWindow_PawnTableWrapper
    { 
        static Action<MainTabWindow_PawnTable> _SetDirty = Dynamic.InstanceVoidMethod<MainTabWindow_PawnTable>("SetDirty");
        public static void SetDirty(this MainTabWindow_PawnTable window) => _SetDirty(window);
    }
}
