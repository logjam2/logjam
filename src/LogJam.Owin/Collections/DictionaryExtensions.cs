// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Extension methods for <see cref="IDictionary" />.
    /// </summary>
    internal static class DictionaryExtensions
    {

        public static T Get<T>(this IDictionary<string, object> dictionary, string key, T fallback = default(T))
        {
            Contract.Requires<ArgumentNullException>(dictionary != null);

            object value;
            return dictionary.TryGetValue(key, out value) ? (T) value : fallback;
        }

    }
}
