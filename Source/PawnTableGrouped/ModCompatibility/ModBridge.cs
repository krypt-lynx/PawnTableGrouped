using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public abstract class ModBridge<T> where T : ModBridge<T>
    {
        private bool disabled = true;

        public static T Instance;

        protected ModBridge()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"Instance of {this.GetType().Name} already initialized");
            }
        }

        public bool IsActive
        {
            get
            {
                return !disabled;
            }
        }

        public void Deactivate()
        {
            disabled = true;
        }

        public static void Resolve(bool active)
        {
            Instance = (T)Activator.CreateInstance(typeof(T));

            Instance.disabled = !active;

            if (Instance.disabled)
            {
                return;
            }

            try
            {
                Instance.disabled = !Instance.ResolveInternal();
            }
            catch
            {
                Instance.disabled = true;
            }
        }

        protected abstract bool ResolveInternal();
    }
}
