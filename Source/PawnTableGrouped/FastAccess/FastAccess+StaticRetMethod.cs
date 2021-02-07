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
        public static Func<TInstance, TResult> StaticRetMethod<TInstance, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TInstance, TResult>(method);
        }

        public static Func<TInstance, TArg0, TResult> StaticRetMethod<TInstance, TArg0, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TInstance, TArg0, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TInstance, TArg0, TArg1, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(method);
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(string methodName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var method = typeof(TInstance).GetMethod(methodName, bindingAttr);
            return StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(method);
        }




        public static Func<TInstance, TResult> StaticRetMethod<TInstance, TResult>(MethodInfo method)
        {
            return (Func<TInstance, TResult>)CreateMethodCaller(typeof(Func<TInstance, TResult>), method, method.ReturnType);
        }

        public static Func<TInstance, TArg0, TResult> StaticRetMethod<TInstance, TArg0, TResult>(MethodInfo method)
        {

            return (Func<TInstance, TArg0, TResult>)CreateMethodCaller(typeof(Func<TInstance, TArg0, TResult>), method, method.ReturnType,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TInstance, TArg0, TArg1, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TResult>(MethodInfo method)
        {
            return (Func<TInstance, TArg0, TArg1, TResult>)CreateMethodCaller(typeof(Func<TInstance, TArg0, TArg1, TResult>), method, method.ReturnType,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TResult>(MethodInfo method)
        {
            return (Func<TInstance, TArg0, TArg1, TArg2, TResult>)CreateMethodCaller(typeof(Func<TInstance, TArg0, TArg1, TArg2, TResult>), method, method.ReturnType,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>(MethodInfo method)
        {
            return (Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>)CreateMethodCaller(typeof(Func<TInstance, TArg0, TArg1, TArg2, TArg3, TResult>), method, method.ReturnType,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }

        public static Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult> StaticRetMethod<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(MethodInfo method)
        {
            return (Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>)CreateMethodCaller(typeof(Func<TInstance, TArg0, TArg1, TArg2, TArg3, TArg4, TResult>), method, method.ReturnType,
                method.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }
}
