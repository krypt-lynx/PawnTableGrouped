using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public interface IWorkTabBridge : IModBridge
    {
        bool ForcePatchWorkTab { get; set; }
        Type WorkTypeWorkerType { get; }
        Type WorkGiverWorkerType { get; }
        bool IsWorkTabWindow(MainTabWindow_PawnTable window);
        bool Expanded(MainTabWindow_PawnTable window);
    }

    public class WorkTabPlaceholderBridge : PlaceholderBridge, IWorkTabBridge
    {     
        public bool ForcePatchWorkTab { get => false; set { } }
        public Type WorkTypeWorkerType => null;
        public Type WorkGiverWorkerType => null;
        public bool IsWorkTabWindow(MainTabWindow_PawnTable window) => false;
        public bool Expanded(MainTabWindow_PawnTable window) => false;
    }
}
