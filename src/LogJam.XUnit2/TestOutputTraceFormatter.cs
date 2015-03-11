// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputTraceFormatter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.XUnit2
{
	using System;
	using System.IO;

	using LogJam.Format;
	using LogJam.Trace;


	/// <summary>
	/// Formats <see cref="TraceEntry"/>s for test output.
	/// </summary>
	public sealed class TestOutputTraceFormatter : LogFormatter<TraceEntry>
	{
		#region Fields

		private static readonly TimeZoneInfo s_outputTimeZone = TimeZoneInfo.Local;

		#endregion

		public TestOutputTraceFormatter()
		{
			IncludeTimeOffset = true;
			StartTimeUtc = DateTime.UtcNow;
		}

		#region Public Properties

		/// <summary>
		/// <c>true</c> to include the Timestamp when formatting <see cref="TraceEntry"/>s.
		/// </summary>
		public bool IncludeTimestamp { get; set; }

		/// <summary>
		/// <c>true</c> to include the time offset (since <see cref="StartTimeUtc"/>)  when formatting <see cref="TraceEntry"/>s.
		/// </summary>
		public bool IncludeTimeOffset { get; set; }

		/// <summary>
		/// The start time to use when computing time offsets.
		/// </summary>
		public DateTime StartTimeUtc { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Formats the trace entry for test output
		/// </summary>
		/// <remarks>The returned string <u>must not</u> end with a newline</remarks>
		public override string Format(ref TraceEntry traceEntry)
		{
			int indentSpaces = 0;

			var sw = new StringWriter();
			var newLine = sw.NewLine;
			var newLineLength = newLine.Length;

			if (IncludeTimeOffset)
			{
				TimeSpan timeOffset = traceEntry.TimestampUtc.Subtract(StartTimeUtc);
				sw.Write("{0:G}  ", timeOffset);
			}

			sw.Write("{0,-7}  ", traceEntry.TraceLevel);

			if (IncludeTimestamp)
			{
#if PORTABLE
				DateTime outputTime = TimeZoneInfo.ConvertTime(timestampUtc, _outputTimeZone);
#else
				DateTime outputTime = TimeZoneInfo.ConvertTimeFromUtc(traceEntry.TimestampUtc, s_outputTimeZone);
#endif
				// TODO: Implement own formatting to make this more efficient
				sw.Write(outputTime.ToString("HH:mm:ss.fff\t"));
			}

			var message = traceEntry.Message.Trim();
			if (message.EndsWith(newLine))
			{
				message = message.Substring(0, message.Length - newLineLength);
			}
			sw.Write("{0,-50}   {1}", traceEntry.TracerName, message);

			if (traceEntry.Details != null)
			{
				//sw.Repeat(' ', indentSpaces);
				string detailsMessage = traceEntry.Details.ToString();
				if (detailsMessage.EndsWith(newLine))
				{
					detailsMessage = detailsMessage.Substring(0, detailsMessage.Length - newLineLength);
				}
				sw.WriteLine();
				sw.Write(detailsMessage);
			}

			return sw.ToString();
		}

		public override void Format(ref TraceEntry traceEntry, TextWriter writer)
		{
			writer.Write(Format(ref traceEntry));
		}

		#endregion
	}

}
