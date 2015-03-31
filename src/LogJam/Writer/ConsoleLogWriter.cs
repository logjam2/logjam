// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriter.cs">
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
	using LogJam.Trace.Format;
	using LogJam.Writer;


	/// <summary>
	/// Logs to the console.
	/// </summary>
	public sealed class ConsoleLogWriter : TextWriterLogWriter, IStartable
	{
		/// <summary>
		/// If more than 5 failures occur while writing to Console.Out, stop writing.
		/// </summary>
		private const short c_maxWriteFailures = 5;

		private short _countWriteFailures;

		public ConsoleLogWriter(ITracerFactory setupTracerFactory, bool useColor, bool synchronize = false)
			: base(Console.Out, setupTracerFactory, synchronize, disposeWriter: false, flushPredicate: BufferingLogWriter.NeverFlush)
		{
			UseColor = useColor;
			_countWriteFailures = 0;
		}

		public bool UseColor { get; set; }

		//public void Write(ref TraceEntry entry)
		//{
		//	if (Enabled)
		//	{
		//		bool useColor = UseColor;
		//		// TODO: Implement coloring

		//		try
		//		{
		//			TextWriter dest;
		//			if (entry.TraceLevel >= TraceLevel.Error)
		//			{
		//				dest = Console.Error;
		//			}
		//			else
		//			{
		//				dest = Console.Out;
		//			}
		//			_formatter.Format(ref entry, dest);
		//		}
		//		catch (Exception)
		//		{
		//			if (++_countWriteFailures > c_maxWriteFailures)
		//			{
		//				Enabled = false;
		//			}
		//			throw;
		//		}
		//	}
		//}

		protected override void InternalStart()
		{
			// Clear the count of write failures if any
			_countWriteFailures = 0;

			base.InternalStart();
		}

	}

}