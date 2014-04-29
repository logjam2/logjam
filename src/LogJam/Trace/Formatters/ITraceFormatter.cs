// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="ITraceFormatter.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Formatters
{
	using System;

	/// <summary>
	/// Given a trace message, an ITraceFormatter formats and returns a text representation of the trace message.
	/// </summary>
	public interface ITraceFormatter
	{
		#region Public Methods and Operators

		/// <summary>
		/// Formats a trace message.
		/// </summary>
		/// <param name="timestampUtc"><see cref="DateTime"/> in UTC that the <see cref="LogJam.Trace.Tracer"/> method was called.</param>
		/// <param name="tracerName">
		/// The tracer name.
		/// </param>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="details">
		/// Trace message details, like an exception.
		/// </param>
		/// <returns>
		/// The formatted trace message.
		/// </returns>
		string FormatTrace(DateTime timestampUtc, string tracerName, TraceLevel traceLevel, string message, object details);

		#endregion
	}
}