using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public class EventBusEventArgs<T> : EventArgs
    {
        public EventBusEventArgs(T Message)
        {
            this.Message = Message;
        }

        public T Message { get; }
    }

    /// <summary>
    /// Weak event bus listener. Allows to observe to be destroyed by Garbadge Collector.
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class EventBusListener<TListener, T> where TListener : class
    {
        Verse.WeakReference<TListener> listener;

        Action<TListener, object, EventBusEventArgs<T>> Action;

        public EventBusListener(TListener listener, Action<TListener, object, EventBusEventArgs<T>> action)
        {
            this.listener = new Verse.WeakReference<TListener>(listener);
            Action = action;
            EventBus<T>.Instance.MessageRecieved += Invoke;
        }

        public void Invoke(object sender, EventBusEventArgs<T> args)
        {
            if (listener.IsAlive)
            {
                Action?.Invoke(listener.Target, sender, args);
            }
            else
            {
                EventBus<T>.Instance.MessageRecieved -= Invoke;
            }
        }
    }

    public class EventBus<T>
    {
        private static EventBus<T> _instance = null;

        //private static readonly object _lock = new object(); // we are in single thread app

        protected EventBus()
        {
        }

        public static EventBus<T> Instance
        {
            get
            {
                //lock (_lock)
                {
                    return _instance ??= new EventBus<T>();
                }
            }
        }

        public event EventHandler<EventBusEventArgs<T>> MessageRecieved;

        public static void SendMessage(object sender, T Message)
        {
            Instance.MessageRecieved?.Invoke(sender, new EventBusEventArgs<T>(Message));
        }

        public void Instance_MessageRecieved(object sender, EventBusEventArgs<T> e)
        {
            throw new NotImplementedException();
        }
    }

}
