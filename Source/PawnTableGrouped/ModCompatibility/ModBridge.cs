using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public abstract class ModBridge<T> where T : ModBridge<T>
    {
        private bool detected = false;
        private bool activated = false;
        //private bool disabled = true;


        public static T Instance;

        protected ModBridge()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"Instance of {this.GetType().Name} already initialized");
            }
        }

        public bool IsDetected => detected;
        public bool IsActive => activated;

        public void Deactivate()
        {
            activated = false;
        }

        public static void Resolve(bool active)
        {
            Instance = (T)Activator.CreateInstance(typeof(T));

            Instance.activated = active;
            Instance.detected = active;

            if (!Instance.activated)
            {
                return;
            }

            try
            {
                Instance.activated = Instance.ResolveInternal();
            }
            catch
            {
                Instance.activated = false;
            }
        }

        protected abstract bool ResolveInternal();
    }
}
