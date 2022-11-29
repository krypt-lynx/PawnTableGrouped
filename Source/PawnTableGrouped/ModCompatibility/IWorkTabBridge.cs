using HarmonyLib;
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
    }

    public class WorkTabPlaceholderBridge : PlaceholderBridge, IWorkTabBridge
    {     
        public bool ForcePatchWorkTab { get => false; set { } }
    }
}
