// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamCollectionExtensions.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Format;
	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// Extension methods for working with <see cref="TraceEntry"/> objects, and collections of <c>TraceEntry</c>s.
	/// </summary>
	public static class LogJamCollectionExtensions
	{

		/// <summary>
		/// Formats and writes <paramref name="entries"/> to <paramref name="textWriter"/>.
		/// </summary>
		/// <param name="entries"></param>
		/// <param name="textWriter"></param>
		/// <param name="logFormatter"></param>
		/// <typeparam name="TEntry"></typeparam>
		public static void WriteEntriesTo<TEntry>(this IEnumerable<TEntry> entries, TextWriter textWriter, LogFormatter<TEntry> logFormatter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(entries != null);
			Contract.Requires<ArgumentNullException>(textWriter != null);
			Contract.Requires<ArgumentNullException>(logFormatter != null);

			var logWriter = new TextWriterLogWriter<TEntry>(textWriter, logFormatter, synchronize: false, disposeWriter: false);
			using (logWriter)
			{
				for (var enumerator = entries.GetEnumerator(); enumerator.MoveNext();)
				{
					TEntry logEntry = enumerator.Current;
					logWriter.Write(ref logEntry);
				}
			}
		}

		/// <summary>
		/// Recursively finds all <see cref="ILogWriter{TEntry}"/> instances with entry type <typeparamref name="TEntry"/>
		/// within the <paramref name="logWriterCollection"/> tree.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="logWriterCollection"></param>
		/// <param name="matches"></param>
		/// <remarks>An enumerable of logWriters acts like a tree, because each <see cref="IMultiLogWriter"/> can contain
		/// multiple <see cref="ILogWriter{TEntry}"/> instances, as well as <see cref="IMultiLogWriter"/> instances.</remarks>
		public static void FindLogWritersByType<TEntry>(this IEnumerable<ILogWriter> logWriterCollection, List<ILogWriter<TEntry>> matches)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(logWriterCollection != null);

			foreach (ILogWriter logWriter in logWriterCollection)
			{
				ILogWriter<TEntry> typedInstance = logWriter as ILogWriter<TEntry>;
				if (typedInstance != null)
				{
					matches.Add(typedInstance);
				}
				else if (logWriter is IMultiLogWriter)
				{	// Recurse
					FindLogWritersByType<TEntry>((IMultiLogWriter) logWriter, matches);
				}
			}

		}

		/// <summary>
		/// Recursively finds all <see cref="ILogWriter{TEntry}"/> instances with entry type <typeparamref name="TEntry"/>
		/// within the <paramref name="logWriterCollection"/> tree, and returns them as a single log writer.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="logWriterCollection"></param>
		/// <param name="logWriter"></param>
		/// <returns><c>true</c> if one or more <see cref="ILogWriter{TEntry}"/> instances were found.  <c>false</c> if no
		/// matching objects were found.</returns>
		/// <remarks>An enumerable of logWriters acts like a tree, because each <see cref="IMultiLogWriter"/> can contain
		/// multiple <see cref="ILogWriter{TEntry}"/> instances, as well as <see cref="IMultiLogWriter"/> instances.</remarks>
		public static bool FindLogWriterByType<TEntry>(this IEnumerable<ILogWriter> logWriterCollection, out ILogWriter<TEntry> logWriter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(logWriterCollection != null);

			var listLogWriters = new List<ILogWriter<TEntry>>();
			FindLogWritersByType(logWriterCollection, listLogWriters);
			if (listLogWriters.Count == 0)
			{
				logWriter = null;
				return false;
			}
			if (listLogWriters.Count == 1)
			{
				logWriter = listLogWriters[0];
			}
			else
			{
				logWriter = new FanOutLogWriter<TEntry>(listLogWriters);
			}
			return true;
		}

	}

}