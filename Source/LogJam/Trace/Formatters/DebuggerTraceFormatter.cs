// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="DebuggerTraceFormatter.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Formatters
{
	using System;
	using System.Text;

	using LogJam.Trace.Util;

	/// <summary>
	/// The debugger trace formatter.
	/// </summary>
	public class DebuggerTraceFormatter : TraceFormatter
	{
		#region Public Methods and Operators

		/// <summary>
		/// The format trace.
		/// </summary>
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
		public override string FormatTrace(string tracerName, TraceLevel traceLevel, string message, Exception exception)
		{
			StringBuilder sb = new StringBuilder(255);
			int indentSpaces = 0;

			//if (TraceManager.Config.ActivityTracingEnabled)
			//{
			//	// Compute indent spaces based on current ActivityRecord scope
			//}

			sb.AppendIndentLines("[", indentSpaces);
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