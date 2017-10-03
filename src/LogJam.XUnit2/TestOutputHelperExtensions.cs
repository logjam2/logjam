// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputHelperExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;
    using System.Collections.Generic;

    using LogJam.Shared.Internal;
    using LogJam.Writer;
    using LogJam.Writer.Text;
    using LogJam.XUnit2;

    using Xunit.Abstractions;


    /// <summary>
    /// Extension methods for easier writing to <see cref="ITestOutputHelper" />.
    /// </summary>
    public static class TestOutputHelperExtensions
    {

        /// <summary>
        /// Formats and writes <paramref name="entries" /> to <paramref name="testOutputHelper" />.
        /// </summary>
        /// <param name="testOutputHelper"></param>
        /// <param name="entries"></param>
        /// <param name="entryFormatter"></param>
        /// <typeparam name="TEntry">The log entry type; must implement <see cref="ILogEntry" /></typeparam>
        public static void WriteEntries<TEntry>(this ITestOutputHelper testOutputHelper, IEnumerable<TEntry> entries, EntryFormatter<TEntry> entryFormatter = null)
            where TEntry : ILogEntry
        {
            Arg.NotNull(testOutputHelper, nameof(testOutputHelper));
            Arg.NotNull(entries, nameof(entries));

            var setupLog = new SetupLog();
            var formatWriter = new TestOutputFormatWriter(testOutputHelper, setupLog);
            var logWriter = new TextLogWriter(setupLog, formatWriter);
            logWriter.AddFormat(entryFormatter);
            logWriter.TryGetEntryWriter(out IEntryWriter<TEntry> entryWriter);
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
