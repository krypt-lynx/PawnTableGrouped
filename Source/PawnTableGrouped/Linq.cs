using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public static class Linq
    {
        public static bool IsUniform<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> selector)
        {
            var e = enumerable.GetEnumerator();
            TValue value;
            if (e.MoveNext())
            {
                value = selector(e.Current);
            }
            else
            {
                return true;
            }

            while (e.MoveNext())
            {
                TValue value2 = selector(e.Current);
                //if (object.Equals(value, value2))
                if (!EqualityComparer<TValue>.Default.Equals(value, value2))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
