using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public partial class FastAccess
    {
        public static Action<TInstance> StaticVoidMethod<TInstance>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TInstance>(method);
        }

        public static Action<TInstance, TArg0> StaticVoidMethod<TInstance, TArg0>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TInstance, TArg0>(method);
        }

        public static Action<TInstance, TArg0, TArg1> StaticVoidMethod<TInstance, TArg0, TArg1>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TInstance, TArg0, TArg1>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TInstance, TArg0, TArg1, TArg2>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(method);
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(method);
        }




        public static Action<TInstance> StaticVoidMethod<TInstance>(MethodInfo method)
        {
            return (Action<TInstance>)CreateMethodCaller(typeof(Action<TInstance>), method, method.ReturnType);
        }

        public static Action<TInstance, TArg0> StaticVoidMethod<TInstance, TArg0>(MethodInfo method)
        {
            return (Action<TInstance, TArg0>)CreateMethodCaller(typeof(Action<TInstance, TArg0>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TInstance, TArg0, TArg1> StaticVoidMethod<TInstance, TArg0, TArg1>(MethodInfo method)
        {
            return (Action<TInstance, TArg0, TArg1>)CreateMethodCaller(typeof(Action<TInstance, TArg0, TArg1>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TInstance, TArg0, TArg1, TArg2> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2>(MethodInfo method)
        {
            return (Action<TInstance, TArg0, TArg1, TArg2>)CreateMethodCaller(typeof(Action<TInstance, TArg0, TArg1, TArg2>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3>(MethodInfo method)
        {
            return (Action<TInstance, TArg0, TArg1, TArg2, TArg3>)CreateMethodCaller(typeof(Action<TInstance, TArg0, TArg1, TArg2, TArg3>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4> StaticVoidMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>(MethodInfo method)
        {
            return (Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>)CreateMethodCaller(typeof(Action<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4>), method, null,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }
}
