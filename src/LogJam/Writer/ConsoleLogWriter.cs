// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Threading;

	using LogJam.Trace;


	/// <summary>
	/// Logs to the console.
	/// </summary>
	public sealed class ConsoleLogWriter : TextLogWriter, IStartable
	{
		/// <summary>
		/// If more than 5 failures occur while writing to Console.Out, stop writing.
		/// </summary>
		private const short c_maxWriteFailures = 5;

		private readonly string _newLine;
		private short _countWriteFailures;

		public ConsoleLogWriter(ITracerFactory setupTracerFactory, bool useColor, bool synchronize = false)
			: base(setupTracerFactory, synchronize)
		{
			UseColor = useColor;
			_countWriteFailures = 0;

			_newLine = Console.Out.NewLine;
		}

		public bool UseColor { get; set; }

		protected override void InternalStart()
		{
			// Clear the count of write failures if any
			_countWriteFailures = 0;

			base.InternalStart();
		}

		protected override void WriteFormattedEntry(string formattedEntry)
		{
			if (!IsStarted)
			{
				return;
			}

			bool lockTaken = false;
			bool includeNewLine = !formattedEntry.EndsWith(_newLine);
			if (synchronize)
			{
				Monitor.Enter(this, ref lockTaken);
			}
			try
			{
				if (includeNewLine)
				{
					Console.Out.WriteLine(formattedEntry);
				}
				else
				{
					Console.Out.Write(formattedEntry);
				}
			}
			catch (Exception excp)
			{
				var setupTracer = SetupTracerFactory.TracerFor(this);
				setupTracer.Error(excp, "Exception caught writing to console");
				if (++_countWriteFailures > c_maxWriteFailures)
				{
					setupTracer.Error("Exceeded {0} write failures, halting console logging.");
					Stop();
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(this);
				}
			}
		}

	}

}