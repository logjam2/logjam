// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
	using System;

	using LogJam.Trace.Formatters;


	/// <summary>
	/// A convenience class for writing to the console.
	/// </summary>
	public sealed class ConsoleTraceWriter : ILogWriter<TraceEntry>
	{

		private bool _disposed;

		// TODO: Stop using formatter if using coloring
		private readonly LogFormatter<TraceEntry> _formatter;

		public ConsoleTraceWriter()
			: this(true)
		{}

		public ConsoleTraceWriter(bool useColor, LogFormatter<TraceEntry> formatter = null)
		{
			UseColor = useColor;

			_formatter = formatter ?? new DebuggerTraceFormatter();
		}

		public bool UseColor { get; set; }

		public bool Enabled { get { return !_disposed; } }

		public bool IsSynchronized { get { return false; } }

		public void Write(ref TraceEntry entry)
		{
			if (!_disposed)
			{
				bool useColor = UseColor;

				// TODO: Implement coloring
				_formatter.Format(ref entry, Console.Out);
			}
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
			}
		}

	}

}