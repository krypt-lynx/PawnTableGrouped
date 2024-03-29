﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public static class Extensions
    {
        /// <summary>
        /// replacement of ternany operator for use in string interpolations
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="condition">condition</param>
        /// <param name="if_">if true result getter</param>
        /// <param name="else_">if false result getter</param>
        /// <returns>condition ? if_() : else_()</returns>
        public static T Case<T>(this bool condition, Func<T> if_, Func<T> else_)
        {
            return condition ? if_() : else_();
        }
    }
}
