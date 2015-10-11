// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputHelperExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


// ReSharper disable CheckNamespace
namespace LogJam
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using LogJam.Format;
    using LogJam.Trace;
    using LogJam.Writer;
    using LogJam.XUnit2;

    using Xunit.Abstractions;


    /// <summary>
    /// Extension methods for easier writing to <see cref="ITestOutputHelper"/>.
    /// </summary>
    public static class TestOutputHelperExtensions
    {

        /// <summary>
        /// Formats and writes <paramref name="entries" /> to <paramref name="testOutputHelper" />.
        /// </summary>
        /// <param name="testOutputHelper"></param>
        /// <param name="entries"></param>
        /// <param name="entryFormatter"></param>
        /// <typeparam name="TEntry">The log entry type; must implement <see cref="ILogEntry"/></typeparam>
        public static void WriteEntries<TEntry>(this ITestOutputHelper testOutputHelper, IEnumerable<TEntry> entries, EntryFormatter<TEntry> entryFormatter = null)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(entries != null);
            Contract.Requires<ArgumentNullException>(testOutputHelper != null);

            if (entryFormatter == null)
            {
                if (typeof(TEntry) == typeof(TraceEntry))
                {
                    entryFormatter = new TestOutputTraceFormatter()
                                   {
                                       IncludeTimestamp = true,
                                       IncludeTimeOffset = false
                                   } as EntryFormatter<TEntry>;
                }
                else
                {
                    // Try creating the default log formatter
                    entryFormatter = DefaultFormatterAttribute.GetDefaultFormatterFor<TEntry>();
                }
                if (entryFormatter == null)
                {
                    throw new ArgumentNullException(nameof(entryFormatter),
                                                    $"No [DefaultFormatter] could be found for entry type {typeof(TEntry).FullName}, so logFormatter argument must be set.");
                }
            }

            var logWriter = new TestOutputLogWriter(testOutputHelper, new SetupLog());
            logWriter.AddFormat(entryFormatter);
            IEntryWriter<TEntry> entryWriter;
            logWriter.TryGetEntryWriter(out entryWriter);
            using (logWriter)
            {
                logWriter.Start();
                for (var enumerator = entries.GetEnumerator(); enumerator.MoveNext();)
                {
                    TEntry logEntry = enumerator.Current;
                    entryWriter.Write(ref logEntry);
                }
            }
        }

    }

}