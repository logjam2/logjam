// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arg.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Shared.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    //#if CODECONTRACTS
    //    
    //#else
    //    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    //    internal class ContractAbbreviatorAttribute: Attribute
    //    { }
    //#endif

    /// <summary>
    /// Argument validation helpers.
    /// </summary>
    internal static class Arg
    {

        [ContractAbbreviator]
        public static void NotNull<T>(T value, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(value != null);
#else
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
#endif
        }

        [ContractAbbreviator]
        public static void NotNullOrEmpty<T>(T[] array, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(array != null);
            Contract.Requires<ArgumentException>(array.Length > 0, $"{parameterName} array must not be empty");
#else
            if (array == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (array.Length == 0)
            {
                throw new ArgumentException($"{parameterName} array must not be empty", paramName: parameterName);
            }
#endif
        }

        [ContractAbbreviator]
        public static void NoneNull<T>(T[] array, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(array != null);
            Contract.Requires<ArgumentException>(array.All(e => e != null), $"{parameterName} array cannot contain any null elements");
#else
            if (array == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == null)
                {
                    throw new ArgumentException($"{parameterName} array cannot contain any null elements", paramName: parameterName);
                }
            }
#endif
        }

        [ContractAbbreviator]
        public static void NotNullOrEmpty<T>(ICollection<T> collection, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentException>(collection.Count > 0, $"{parameterName} collection must not be empty");
#else
            if (collection == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (collection.Count == 0)
            {
                throw new ArgumentException($"{parameterName} collection must not be empty", paramName: parameterName);
            }
#endif
        }

        [ContractAbbreviator]
        public static void NoneNull<T>(IEnumerable<T> collection, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentException>(collection.All(e => e != null), $"{parameterName} collection cannot contain any null elements");
#else
            if (collection == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            foreach (T t in collection)
            {
                if (t == null)
                {
                    throw new ArgumentException($"{parameterName} collection cannot contain any null elements", paramName: parameterName);
                }
            }
#endif
        }

        [ContractAbbreviator]
        public static void NotNullOrEmpty(string value, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Requires<ArgumentException>(value.Length > 0, $"{parameterName} string must not be empty");
#else
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (value.Length == 0)
            {
                throw new ArgumentException($"{parameterName} string must not be empty", paramName: parameterName);
            }
#endif
        }

        [ContractAbbreviator]
        public static void NotNullOrWhitespace(string value, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(value), $"The argument for parameter {parameterName} must not be all whitespace.");
#else
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (! char.IsWhiteSpace(value[i]))
                {
                    throw new ArgumentException($"The argument for parameter {parameterName} must not be all whitespace.", paramName: parameterName);
                }
            }
#endif
        }

        [ContractAbbreviator]
        public static void InRange(int value, int minimum, int maximum, string parameterName)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentOutOfRangeException>(value >= minimum, $"{parameterName} value {value} cannot be less than {minimum}.");
            Contract.Requires<ArgumentOutOfRangeException>(value <= maximum, $"{parameterName} value {value} cannot be greater than {maximum}.");
#else
            if (value < minimum)
            {
                throw new ArgumentOutOfRangeException(paramName: parameterName, actualValue: value, message: $"{parameterName} value {value} cannot be less than {minimum}.");
            }
            if (value > maximum)
            {
                throw new ArgumentOutOfRangeException(paramName: parameterName, actualValue: value, message: $"{parameterName} value {value} cannot be greater than {maximum}.");
            }
#endif
        }

    }
}
