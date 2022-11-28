using RimWorld;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    class MainTabWindow_PawnTableWrapper
    { 
        Verse.WeakReference<MainTabWindow_PawnTable> window;

        public MainTabWindow_PawnTableWrapper(MainTabWindow_PawnTable window)
        {
            this.window = new Verse.WeakReference<MainTabWindow_PawnTable>(window);
        }

        public MainTabWindow_PawnTable Window => window.Target;

        static Action<MainTabWindow_PawnTable> _SetDirty = Dynamic.InstanceVoidMethod<MainTabWindow_PawnTable>("SetDirty");
        public void SetDirty() => _SetDirty(Window);
    }
}
