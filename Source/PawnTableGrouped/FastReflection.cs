using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public class FastReflection
    {    
        public static Func<TObject, TResult> CreateCallStaticMethodDelegate<TObject, TResult>(MethodInfo method)
        {
            DynamicMethod wrapper = new DynamicMethod($"call_{method.DeclaringType.Name}_{method.Name}", typeof(TResult), new Type[] { typeof(TObject) }, true);
            ILGenerator gen = wrapper.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException($"method {method} is not static");
            }

            return (Func<TObject, TResult>)wrapper.CreateDelegate(typeof(Func<TObject, TResult>));
        }

    }
}
