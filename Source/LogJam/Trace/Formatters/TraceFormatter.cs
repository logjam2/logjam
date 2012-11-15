// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="TraceFormatter.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Formatters
{
	using System;

	/// <summary>
	/// Base class for trace formatters.  Given a trace message, a TraceFormatter formats and returns a text representation 
	/// to render the trace message in text.
	/// </summary>
	public abstract class TraceFormatter
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
		public abstract string FormatTrace(string tracerName, TraceLevel traceLevel, string message, Exception exception);

		#endregion
	}
}