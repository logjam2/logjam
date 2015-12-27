// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamCollectionExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    using LogJam.Trace;
    using LogJam.Writer;
    using LogJam.Writer.Text;


    /// <summary>
    /// Extension methods for working with <see cref="TraceEntry" /> objects, and collections of <c>TraceEntry</c>s.
    /// </summary>
    public static class LogJamCollectionExtensions
    {

        /// <summary>
        /// Formats and writes <paramref name="entries" /> to <paramref name="textWriter" />.
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="textWriter"></param>
        /// <param name="entryFormatter"></param>
        /// <typeparam name="TEntry"></typeparam>
        public static void WriteEntriesTo<TEntry>(this IEnumerable<TEntry> entries, TextWriter textWriter, EntryFormatter<TEntry> entryFormatter = null)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(entries != null);
            Contract.Requires<ArgumentNullException>(textWriter != null);

            if (entryFormatter == null)
            {   // Try creating the default log formatter
                entryFormatter = DefaultFormatterAttribute.GetDefaultFormatterFor<TEntry>();
                if (entryFormatter == null)
                {
                    throw new ArgumentNullException(nameof(entryFormatter),
                                                    $"No [DefaultFormatter] could be found for entry type {typeof(TEntry).FullName}, so logFormatter argument must be set.");
                }
            }

            var setupLog = new SetupLog();
            var formatWriter = new TextWriterFormatWriter(setupLog, textWriter, disposeWriter: false);
            var logWriter = new TextLogWriter(setupLog, formatWriter);
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

        /// <summary>
        /// Returns <c>true</c> if the <paramref name="traceEntries" /> collection contains any <see cref="TraceEntry" /> instances
        /// with <see cref="TraceEntry.TraceLevel" /> exceeding <paramref name="traceLevel" />.
        /// </summary>
        /// <param name="traceEntries"></param>
        /// <param name="traceLevel"></param>
        /// <returns></returns>
        public static bool HasAnyExceeding(this IEnumerable<TraceEntry> traceEntries, TraceLevel traceLevel)
        {
            return traceEntries.Any(traceEntry => traceEntry.TraceLevel > traceLevel);
        }

        /// <summary>
        /// Finds all <see cref="IEntryWriter{TEntry}" /> instances with entry type <typeparamref name="TEntry" />
        /// within the <paramref name="logWriterCollection" />.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="logWriterCollection"></param>
        /// <param name="entryWriters">Matching <see cref="IEntryWriter{TEntry}" />s are added to this list.</param>
        public static void GetEntryWriters<TEntry>(this IEnumerable<ILogWriter> logWriterCollection, List<IEntryWriter<TEntry>> entryWriters)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(logWriterCollection != null);

            foreach (ILogWriter logWriter in logWriterCollection)
            {
                IEntryWriter<TEntry> entryWriter;
                if (logWriter.TryGetEntryWriter(out entryWriter))
                {
                    entryWriters.Add(entryWriter);
                }
            }

        }

        /// <summary>
        /// Finds all <see cref="IEntryWriter{TEntry}" /> instances with entry type <typeparamref name="TEntry" />
        /// within the <paramref name="logWriterCollection" />, and returns them as a single entry writer.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="logWriterCollection"></param>
        /// <param name="entryWriter"></param>
        /// <returns>
        /// <c>true</c> if one or more <see cref="IEntryWriter{TEntry}" /> instances were found.  <c>false</c> if no
        /// matching objects were found.
        /// </returns>
        public static bool GetSingleEntryWriter<TEntry>(this IEnumerable<ILogWriter> logWriterCollection, out IEntryWriter<TEntry> entryWriter)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(logWriterCollection != null);

            var listLogWriters = new List<IEntryWriter<TEntry>>();
            GetEntryWriters(logWriterCollection, listLogWriters);
            if (listLogWriters.Count == 0)
            {
                entryWriter = null;
                return false;
            }
            if (listLogWriters.Count == 1)
            {
                entryWriter = listLogWriters[0];
            }
            else
            {
                entryWriter = new FanOutEntryWriter<TEntry>(listLogWriters);
            }
            return true;
        }

        /// <summary>
        /// Coerces a collection of <see cref="IEntryWriter{TEntry}" /> instances with entry type <typeparamref name="TEntry" />
        /// into a single entry writer.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entryWriterCollection"></param>
        /// <returns>
        /// <c>true</c> if one or more <see cref="IEntryWriter{TEntry}" /> instances were found.  <c>false</c> if no
        /// matching objects were found.
        /// </returns>
        public static IEntryWriter<TEntry> GetSingleEntryWriter<TEntry>(this IEnumerable<IEntryWriter<TEntry>> entryWriterCollection)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(entryWriterCollection != null);

            int count = entryWriterCollection.Count();
            if (count == 0)
            {
                return new NoOpEntryWriter<TEntry>();
            }
            if (count == 1)
            {
                return entryWriterCollection.First();
            }
            else
            {
                return new FanOutEntryWriter<TEntry>(entryWriterCollection);
            }
        }

    }

}
