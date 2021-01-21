using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public abstract class ModBridge
    {
        protected bool detected = false;
        protected bool activated = false;

        public bool IsDetected => detected;
        public bool IsActive => activated;

        public void Deactivate()
        {
            activated = false;
        }

        protected ModBridge() { }

        public void Resolve(bool active, Harmony harmony)
        {
            activated = active;
            detected = active;

            if (!activated)
            {
                return;
            }

            try
            {
                var success = this.ResolveInternal(harmony);
                activated |= success;

                ApplyPatches(harmony);
            }
            catch
            {
                activated = false;
                $"{this.GetType()} was activated but failed initialization, integration disabled".Log(MessageType.Warning);
            }
        }

        protected abstract bool ResolveInternal(Harmony harmony);
        protected virtual void ApplyPatches(Harmony harmony) { }
    }

    public abstract class ModBridge<T> : ModBridge where T : ModBridge, new()
    {

        private static T instance = null;
        public static T Instance => instance ??= new T();

        protected ModBridge()
        {
            if (instance != null)
            {
                throw new InvalidOperationException($"Instance of {this.GetType().Name} already initialized");
            }
            else
            {
                instance = (T)(object)this;
            }
        }
    }
}
