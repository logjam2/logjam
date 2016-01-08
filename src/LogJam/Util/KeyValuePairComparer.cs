// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyValuePairComparer.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
    using System.Collections.Generic;


    /// <summary>
    /// An <see cref="IEqualityComparer{T}" /> and <see cref="IComparer{T}" /> for <see cref="KeyValuePair{TKey,TValue}" />.
    /// </summary>
    internal sealed class KeyValuePairComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>, IComparer<KeyValuePair<TKey, TValue>>
    {

        public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return Equals(x.Key, y.Key) && Equals(x.Value, y.Value);
        }

        public int GetHashCode(KeyValuePair<TKey, TValue> obj)
        {
            return EqualityUtil.CombineHashCodes(obj.Key.Hash(), obj.Value.Hash());
        }

        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            int c = Comparer<TKey>.Default.Compare(x.Key, y.Key);
            if (c == 0)
            {
                c = Comparer<TValue>.Default.Compare(x.Value, y.Value);
            }
            return c;
        }

    }

}
