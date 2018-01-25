// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListDictionary.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;

using LogJam.Shared.Internal;

namespace LogJam.Util.Collections
{

    /// <summary>
    /// Implements <see cref="IDictionary{TKey,TValue}" /> using a list. It's not efficient, but it preserves the order of key
    /// value pairs.
    /// </summary>
    /// <remarks>
    /// Unlike a normal <c>Dictionary</c>, order matters when determining equality. This is more like a list of
    /// <see cref="KeyValuePair{TKey,TValue}" />s,
    /// but with support for <see cref="IDictionary{TKey,TValue}" /> operations.
    /// <para>This collection is not thread-safe.</para>
    /// </remarks>
    public class ListDictionary<TKey, TValue> : List<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IEquatable<IDictionary<TKey, TValue>>
    {

        private static readonly KeyValuePairComparer<TKey, TValue> s_comparer = new KeyValuePairComparer<TKey, TValue>();

        public bool Equals(IDictionary<TKey, TValue> other)
        {
            if (other == null)
                return false;

            return this.SequenceEqual(other, s_comparer);
        }

        /// <summary>
        /// Returns the index of the element with key equal to <paramref name="key" />.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOf(TKey key)
        {
            Arg.NotNull(key, nameof(key));

            return FindIndex(kvp => Equals(kvp.Key, key));
        }

        private void RequireNotNull(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ListDictionary<TKey, TValue>);
        }

        public override int GetHashCode()
        {
            return this.GetOrderedDictionaryHashCode();
        }

        #region IDictionary

        public bool ContainsKey(TKey key)
        {
            RequireNotNull(key);

            return this.Any(kvp => Equals(kvp.Key, key));
        }

        public void Add(TKey key, TValue value)
        {
            RequireNotNull(key);

            if (ContainsKey(key))
                throw new ArgumentException("Key already exists in ListDictionary.", "key");
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(TKey key)
        {
            RequireNotNull(key);

            int index = IndexOf(key);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            RequireNotNull(key);

            int index = IndexOf(key);
            if (index >= 0)
            {
                value = this[index].Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                RequireNotNull(key);

                int index = IndexOf(key);
                if (index >= 0)
                    return this[index].Value;
                else
                    throw new KeyNotFoundException("Key not found");
            }
            set
            {
                RequireNotNull(key);

                int index = IndexOf(key);
                if (index >= 0)
                    this[index] = new KeyValuePair<TKey, TValue>(key, value);
                else
                    Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        public ICollection<TKey> Keys { get { return this.Select(kvp => kvp.Key).ToArray(); } }

        public ICollection<TValue> Values { get { return this.Select(kvp => kvp.Value).ToArray(); } }

        #endregion

    }

}
