using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public interface ISettingsWorker
    {
        void TableActiveChanged(string tableName, bool active);
    }

    public interface IModBridge
    {
        bool IsDetected { get; }
        bool IsActive { get; }

        void Deactivate();
        string ModName();

        void Resolve(bool active, Harmony harmony);
    }


    public abstract class ModBridge: IModBridge
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

        public abstract string ModName();
        
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
            catch (Exception e)
            {
                activated = false;
                LogHelper.LogException($"{this.GetType()} was activated but failed initialization, integration disabled", e);
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

    public class PlaceholderBridge : IModBridge
    {
        public bool IsDetected => false;
        public bool IsActive => false;

        public void Deactivate() { }
        public string ModName() => "placeholder (you are not supposed to see this)";
        public void Resolve(bool active, Harmony harmony) { }
    }
}
