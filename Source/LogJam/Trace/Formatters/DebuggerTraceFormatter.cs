// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerTraceFormatter.cs" company="Crim Consulting">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Formatters
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Text;

	using LogJam.Trace.Util;

	/// <summary>
	/// The debugger trace formatter.
	/// </summary>
	public class DebuggerTraceFormatter : ITraceFormatter
	{
		#region Fields

		private TimeZoneInfo _outputTimeZone = TimeZoneInfo.Local;

		#endregion

		#region Public Properties

		public bool IncludeTimestamp { get; set; }

		public TimeZoneInfo OutputTimeZone
		{
			get
			{
				return _outputTimeZone;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				_outputTimeZone = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The format trace.
		/// </summary>
		/// <param name="timestampUtc"></param>
		/// <param name="tracerName">
		/// The tracer name.
		/// </param>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string FormatTrace(DateTime timestampUtc, string tracerName, TraceLevel traceLevel, string message, Exception exception)
		{
			StringBuilder sb = new StringBuilder(255);
			int indentSpaces = 0;

			//if (TraceManager.Config.ActivityTracingEnabled)
			//{
			//	// Compute indent spaces based on current ActivityRecord scope
			//}

			sb.Append(' ', indentSpaces);

			if (IncludeTimestamp)
			{
#if PORTABLE
				DateTime outputTime = TimeZoneInfo.ConvertTime(timestampUtc, _outputTimeZone);
#else
				DateTime outputTime = TimeZoneInfo.ConvertTimeFromUtc(timestampUtc, _outputTimeZone);
#endif
				// TODO: Implement own formatting to make this more efficient
				sb.Append(outputTime.ToString("HH:mm:ss.fff "));
			}

			sb.Append('[');
			sb.Append(traceLevel);
			sb.Append("] ");
			sb.Append(tracerName);
			sb.Append(" \t");

			sb.Append(message);
			sb.EnsureEndsWith(Environment.NewLine);

			if (exception != null)
			{
				sb.AppendIndentLines(exception.ToString(), indentSpaces);
				sb.EnsureEndsWith(Environment.NewLine);
			}

			return sb.ToString();
		}

		#endregion
	}
}