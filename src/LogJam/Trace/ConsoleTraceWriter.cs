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
	using System.IO;

	using LogJam.Format;
	using LogJam.Trace.Formatters;


	/// <summary>
	/// A convenience class for writing to the console.
	/// </summary>
	public sealed class ConsoleTraceWriter : ILogWriter<TraceEntry>, IStartable
	{
		/// <summary>
		/// If more than 5 failures occur while writing to Console.Out, stop writing.
		/// </summary>
		private const short c_maxWriteFailures = 5;

		private short _countWriteFailures;

		// TODO: Stop using formatter if using coloring
		private readonly LogFormatter<TraceEntry> _formatter;

		public ConsoleTraceWriter()
			: this(true)
		{}

		public ConsoleTraceWriter(bool useColor, LogFormatter<TraceEntry> formatter = null)
		{
			UseColor = useColor;

			_formatter = formatter ?? new DebuggerTraceFormatter();
			_countWriteFailures = 0;
			Enabled = true;
		}

		public bool UseColor { get; set; }

		public bool Enabled { get; set; }

		public bool IsSynchronized { get { return false; } }

		public void Write(ref TraceEntry entry)
		{
			if (Enabled)
			{
				bool useColor = UseColor;
				// TODO: Implement coloring

				try
				{
					TextWriter dest;
					if (entry.TraceLevel >= TraceLevel.Error)
					{
						dest = Console.Error;
					}
					else
					{
						dest = Console.Out;
					}
					_formatter.Format(ref entry, dest);
				}
				catch (Exception)
				{
					if (++_countWriteFailures > c_maxWriteFailures)
					{
						Enabled = false;
					}
					throw;
				}
			}
		}

		public void Start()
		{
			// Clear the count of write failures if any
			_countWriteFailures = 0;
			Enabled = true;
		}

		public void Stop()
		{
			Enabled = false;
		}

		public bool IsStarted { get { return Enabled; } }

	}

}