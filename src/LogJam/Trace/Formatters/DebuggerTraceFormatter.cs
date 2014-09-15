// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerTraceFormatter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Formatters
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Text;

	using LogJam.Util.Text;


	/// <summary>
	/// The debugger trace formatter.
	/// </summary>
	public class DebuggerTraceFormatter : LogFormatter<TraceEntry>
	{
		#region Fields

		private TimeZoneInfo _outputTimeZone = TimeZoneInfo.Local;

		#endregion

		public DebuggerTraceFormatter()
		{}

		#region Public Properties

		/// <summary>
		/// <c>true</c> to include the Timestamp when formatting <see cref="TraceEntry"/>s.
		/// </summary>
		public bool IncludeTimestamp { get; set; }

		/// <summary>
		/// Specifies the TimeZone to use when formatting the Timestamp for a <see cref="TraceEntry"/>.
		/// </summary>
		public TimeZoneInfo OutputTimeZone
		{
			get { return _outputTimeZone; }
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				_outputTimeZone = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Formats the trace entry for debugger windows
		/// </summary>
		public override void Format(ref TraceEntry traceEntry, TextWriter writer)
		{
			int indentSpaces = 0;

			var sw = new StringWriter();
			//if (TraceManager.Config.ActivityTracingEnabled)
			//{
			//	// Compute indent spaces based on current ActivityRecord scope
			//}

			sw.Repeat(' ', indentSpaces);

			sw.Write("{0,-7}\t", traceEntry.TraceLevel);

			if (IncludeTimestamp)
			{
#if PORTABLE
				DateTime outputTime = TimeZoneInfo.ConvertTime(timestampUtc, _outputTimeZone);
#else
				DateTime outputTime = TimeZoneInfo.ConvertTimeFromUtc(traceEntry.TimestampUtc, _outputTimeZone);
#endif
				// TODO: Implement own formatting to make this more efficient
				sw.Write(outputTime.ToString("HH:mm:ss.fff\t"));
			}

			var message = traceEntry.Message.Trim();
			sw.Write("{0,-50}     {1}", traceEntry.TracerName, message);
			if (! message.EndsWith(writer.NewLine))
			{
				sw.WriteLine();
			}

			if (traceEntry.Details != null)
			{
				sw.Repeat(' ', indentSpaces);
				string detailsMessage = traceEntry.Details.ToString();
				sw.Write(detailsMessage);
				if (! detailsMessage.EndsWith(writer.NewLine))
				{
					sw.WriteLine();
				}
			}

			writer.Write(sw);
		}

		#endregion
	}
}
