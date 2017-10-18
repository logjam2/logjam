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
    using System.Diagnostics;
#if CODECONTRACTS
    using System.Diagnostics.Contracts;
    using System.Linq;
#endif

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

        [Conditional("DEBUG")]
        [ContractAbbreviator]
        public static void DebugNotNull<T>(T value, string parameterName)
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

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="value"/> is empty or all whitespace.
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
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
            int i = 0;
            for (; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    break;
                }
            }
            if (i == value.Length)
            {
                throw new ArgumentException($"The argument for parameter {parameterName} must not be empty or all whitespace.", paramName: parameterName);
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

        [ContractAbbreviator]
        public static void Requires(bool assertion, string assertionDescription)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentException>(assertion, assertionDescription);
#else
            if (!assertion)
            {
                throw new ArgumentException(assertionDescription);
            }
#endif
        }

        [ContractAbbreviator]
        [Conditional("DEBUG")]
        public static void DebugRequires(bool assertion, string assertionDescription)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentException>(assertion, assertionDescription);
#else
            if (!assertion)
            {
                throw new ArgumentException(assertionDescription);
            }
#endif
        }

        /// <summary>
        /// Verifies that <paramref name="value"/> is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message"></param>
        [ContractAbbreviator]
        public static void Is<T>(object value, string parameterName, string message = null)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentException>((value is T), message);
#else
            if (!(value is T))
            {
                if (message != null)
                {
                    throw new ArgumentException($"Must be type {typeof(T).FullName}", paramName: parameterName);
                }
                else
                {
                    throw new ArgumentException(message, paramName: parameterName);
                }
            }
#endif
        }

        /// <summary>
        /// Verifies that <paramref name="value"/> is NOT of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message"></param>
        [ContractAbbreviator]
        public static void IsNot<T>(object value, string parameterName, string message = null)
        {
#if CODECONTRACTS
            Contract.Requires<ArgumentException>((value is T), message);
#else
            if ((value is T))
            {
                if (message != null)
                {
                    throw new ArgumentException($"Must not be type {typeof(T).FullName}", paramName: parameterName);
                }
                else
                {
                    throw new ArgumentException(message, paramName: parameterName);
                }
            }
#endif
        }

    }
}
