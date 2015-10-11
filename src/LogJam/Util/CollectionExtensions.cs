// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// Extension methods for collection classes
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// Returns <c>true</c> if <paramref name="collection1" /> and <paramref name="collection2" /> contain equal elements.
        /// The order of elements doesn't matter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection1"></param>
        /// <param name="collection2"></param>
        /// <returns></returns>
        public static bool CollectionEqual<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            if (collection1 == null)
            {
                if (collection2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (collection2 == null)
            {
                return false;
            }

            var tempList2 = collection2.ToList();
            foreach (var t in collection1)
            {
                if (! tempList2.Remove(t))
                {
                    return false;
                }
            }

            return tempList2.Count == 0;
        }

        public static bool DictionaryEqual<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict1, IDictionary<TKey, TValue> dict2)
        {
            if (dict1 == null)
            {
                if (dict2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (dict2 == null)
            {
                return false;
            }

            var tempDict2 = new Dictionary<TKey, TValue>(dict2);
            foreach (var kvp in dict1)
            {
                TValue value;
                if (! tempDict2.TryGetValue(kvp.Key, out value)
                    || ! Equals(value, kvp.Value))
                {
                    return false;
                }
                tempDict2.Remove(kvp.Key);
            }

            return tempDict2.Count == 0;
        }

        public static int GetOrderedCollectionHashCode<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return 0;
            }

            int hashCode = 0;
            foreach (T t in collection)
            {
                hashCode = EqualityUtil.CombineHashCodes(hashCode, t.GetHashCode());
            }
            return hashCode;
        }

        public static int GetUnorderedCollectionHashCode<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return 0;
            }

            int hashCode = 0;
            foreach (T t in collection)
            {
                hashCode ^= t.GetHashCode();
            }
            return hashCode;
        }

        public static int GetOrderedDictionaryHashCode<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
        {
            if (dictionary == null)
            {
                return 0;
            }

            int hashCode = 0;
            foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                hashCode = EqualityUtil.CombineHashCodes(hashCode, EqualityUtil.CombineHashCodes(kvp.Key.Hash(), kvp.Value.Hash()));
            }
            return hashCode;
        }

    }

}
