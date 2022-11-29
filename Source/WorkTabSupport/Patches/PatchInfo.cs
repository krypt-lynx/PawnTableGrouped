using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public struct PatchInfo
    {
        public bool NormallyEnabled;
        public MethodBase Target;
        public HarmonyMethod Prefix;
        public HarmonyMethod Postfix;
        public HarmonyMethod Transpiler;
        public HarmonyMethod Finalizer;

        public PatchInfo(bool normallyEnabled, MethodBase target = null, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
        {
            NormallyEnabled = normallyEnabled;
            Target = target;
            Prefix = prefix;
            Postfix = postfix;
            Transpiler = transpiler;
            Finalizer = finalizer;
        }
    }

    public class PatchesBatch
    {
        PatchInfo[] Patches;
        Harmony Harmony;

        public PatchesBatch(PatchInfo[] patches, Harmony harmony)
        {
            Patches = patches;
            Harmony = harmony;
        }

        bool enabled = false;
        public bool Enabled
        {
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    UpdatePatches(value);
                } 
            }
        }

        private void UpdatePatches(bool enabled)
        {
            foreach (var patch in Patches)
            {
                if (patch.NormallyEnabled != enabled)
                {
                    Harmony.Patch(patch.Target, patch.Prefix, patch.Postfix, patch.Transpiler, patch.Finalizer);
                }
                else
                {
                    if (patch.Prefix != null) 
                    {
                        Harmony.Unpatch(patch.Target, HarmonyPatchType.Prefix, Harmony.Id);
                    }
                    if (patch.Postfix != null)
                    {
                        Harmony.Unpatch(patch.Target, HarmonyPatchType.Postfix, Harmony.Id);
                    }
                    if (patch.Transpiler != null)
                    {
                        Harmony.Unpatch(patch.Target, HarmonyPatchType.Transpiler, Harmony.Id);
                    }
                    if (patch.Finalizer != null)
                    {
                        Harmony.Unpatch(patch.Target, HarmonyPatchType.Finalizer, Harmony.Id);
                    }
                }
            }
        }
    }

}
