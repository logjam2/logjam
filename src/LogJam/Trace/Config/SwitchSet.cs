// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchSet.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
    using System;
    using System.Reflection;

    using LogJam.Shared.Internal;
    using LogJam.Util;


    /// <summary>
    /// Holds the switches for a <see cref="TraceWriterConfig" /> as a set of key-value pairs,
    /// where the key is a name prefix, and the value is a <see cref="ITraceSwitch" />.
    /// </summary>
    public sealed class SwitchSet : ListDictionary<string, ITraceSwitch>
    {

        /// <summary>
        /// Finds the <see cref="ITraceSwitch" /> in this list that containsprefix matches <paramref name="tracerName" />.
        /// No matching switch may be found, in which case <c>false</c> is returned.
        /// </summary>
        /// <param name="tracerName"></param>
        /// <param name="traceSwitch"></param>
        /// <returns></returns>
        public bool FindBestMatchingSwitch(string tracerName, out ITraceSwitch traceSwitch)
        {
            int bestMatchLength = -1;
            traceSwitch = null;

            foreach (var kvp in this)
            {
                if (kvp.Key.Length > bestMatchLength)
                {
                    if (tracerName.StartsWith(kvp.Key) && (kvp.Value != null))
                    {
                        bestMatchLength = kvp.Key.Length;
                        traceSwitch = kvp.Value;
                    }
                }
            }

            return bestMatchLength >= 0;
        }

        /// <summary>
        /// Adds a <paramref name="traceSwitch" /> associated with the specified <paramref name="tracerType" />.
        /// </summary>
        /// <param name="tracerType">The <see cref="Type" /> to associate <paramref name="traceSwitch" /> with.</param>
        /// <param name="traceSwitch">An <see cref="ITraceSwitch" />.</param>
        public void Add(Type tracerType, ITraceSwitch traceSwitch)
        {
            Arg.NotNull(tracerType, nameof(tracerType));
            Arg.NotNull(traceSwitch, nameof(traceSwitch));

            string tracerName = tracerType.GetCSharpName();
            if (tracerType.GetTypeInfo().IsGenericTypeDefinition)
            { // Remove everything after the open generic bracket - to match all generic types.
                int ichBracket = tracerName.IndexOf('<');
                if (ichBracket > 0)
                {
                    tracerName = tracerName.Substring(0, ichBracket + 1);
                }
            }

            Add(tracerName, traceSwitch);
        }

    }

}
