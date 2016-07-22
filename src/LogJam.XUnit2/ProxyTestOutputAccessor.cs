// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyTestOutputAccessor.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
    using System;
    using System.Collections.Generic;

    using Xunit.Abstractions;


    /// <summary>
    /// An <see cref="ITestOutputAccessor"/> that can be changed to point at other <see cref="ITestOutputAccessor"/>s.
    /// </summary>
    internal sealed class ProxyTestOutputAccessor : ITestOutputAccessor
    {

        private readonly List<WeakReference<ITestOutputAccessor>> _targets = new List<WeakReference<ITestOutputAccessor>>();

        /// <summary>
        /// Adds <paramref name="proxyTarget"/> to the list of proxied targets.
        /// </summary>
        /// <param name="proxyTarget"></param>
        public void AddTarget(ITestOutputAccessor proxyTarget)
        {
            _targets.Add(new WeakReference<ITestOutputAccessor>(proxyTarget));
        }

        /// <summary>
        /// Removes <paramref name="proxyTarget"/> from the list of proxied targets.
        /// </summary>
        /// <param name="proxyTarget"></param>
        /// <returns>The number of elements removed.</returns>
        public int RemoveTarget(ITestOutputAccessor proxyTarget)
        {
            int matches = 0;
            _targets.RemoveAll(wr =>
                               {
                                   ITestOutputAccessor accessor;
                                   if (! wr.TryGetTarget(out accessor))
                                   {
                                       return true;
                                   }
                                   if (ReferenceEquals(accessor, proxyTarget))
                                   {
                                       matches++;
                                       return true;
                                   }
                                   return false;
                               });
            return matches;
        }

        /// <summary>
        /// Sets the <see cref="ITestOutputHelper" /> to use to send log output to.
        /// </summary>
        public ITestOutputHelper TestOutput
        {
            set
            {
                for (int i = _targets.Count - 1; i >= 0; --i)
                {
                    var weakRef = _targets[i];
                    ITestOutputAccessor targetOutputAccessor;
                    if (weakRef.TryGetTarget(out targetOutputAccessor))
                    {
                        targetOutputAccessor.TestOutput = value;
                    }
                    else
                    {
                        _targets.RemoveAt(i);
                    }
                }
            }
        }

    }

}