// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualityUtil.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
    using System.Collections;
    using System.Collections.Generic;


    /// <summary>
    /// Reusable methods for equality comparison and hash code computation.
    /// </summary>
    internal static class EqualityUtil
    {

        /// <summary>
        /// Hashes a variable using its <see cref="object.GetHashCode" /> implementation, but tolerates nulls.  Also left-shifts by
        /// <paramref name="leftShift" />.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="leftShift"></param>
        /// <returns></returns>
        public static int Hash(this object o, int leftShift = 0)
        {
            return o == null ? 0 : (o.GetHashCode() << leftShift);
        }

        /// <summary>
        /// Combines 2 hash codes into 1.
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="e1" /> and <paramref name="e2" /> contain the same quantity
        /// of equal elements.  The order of the elements within the enumerable doesn't matter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        internal static bool AreEquivalent<T>(IEnumerable<T> e1, IEnumerable<T> e2)
        {
            if (e1 == null)
            {
                return e2 == null;
            }
            if (e2 == null)
            {
                return false;
            }

            // Find count if possible, but don't assume we can enumerate twice
            //  (which is why Enumerable.Count() isn't called)
            int e1Count = 20; // Default Dictionary size
            ICollection<T> collection1 = e1 as ICollection<T>;
            if (collection1 != null)
            {
                e1Count = collection1.Count;
                ICollection collection2 = e2 as ICollection;
                if ((collection2 != null) &&
                    (collection2.Count != e1Count))
                {
                    return false;
                }
            }

            // Map of instances to unmatched copies
            Dictionary<T, int> d = new Dictionary<T, int>(e1Count);

            // Add count of copies for everything in e1
            foreach (T t in e1)
            {
                int copies;
                if (! d.TryGetValue(t, out copies))
                {
                    d[t] = 1;
                }
                else
                {
                    d[t] = copies + 1;
                }
            }

            // Subtract count of copies for everything in e2
            foreach (T t in e2)
            {
                int unmatchedCopies;
                if (! d.TryGetValue(t, out unmatchedCopies))
                {
                    return false;
                }
                else
                {
                    if (unmatchedCopies < 1)
                    {
                        return false;
                    }
                    d[t] = unmatchedCopies - 1;
                }
            }

            foreach (var kvp in d)
            {
                if (kvp.Value != 0)
                {
                    return false;
                }
            }

            return true;
        }

    }

}
