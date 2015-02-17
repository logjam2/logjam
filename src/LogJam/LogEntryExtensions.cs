// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntryExtensions.cs">
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
	public static class LogEntryExtensions
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

			var logWriter = new TextWriterLogWriter<TEntry>(textWriter, logFormatter, autoFlush: false, disposeWriter: false);
			using (logWriter)
			{
				for (var enumerator = entries.GetEnumerator(); enumerator.MoveNext();)
				{
					TEntry logEntry = enumerator.Current;
					logWriter.Write(ref logEntry);
				}
			}
		}

	}

}