// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="ITraceCollector.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Collectors
{
	using System;

	/// <summary>
	/// First collection point for trace messages created by <see cref="Tracer"/> instances.  All <c>Tracer</c> instances
	/// forward their trace messages to an object that implements <c>ITraceCollector</c>.
	/// </summary>
	/// <remarks>
	/// <c>ITraceCollector</c> implementations must be thread-safe.
	/// </remarks>
	public interface ITraceCollector
	{
		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="ITraceCollector"/> should receive <see cref="Tracer"/> messages.
		/// </summary>
		/// <value>
		/// If <c>true</c>, this <c>ITraceCollector</c> should receive messages.  If <c>false</c>, none of the other 
		/// <c>ITraceCollector</c> methods should be called.
		/// </value>
		bool IsActive { get; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The append message.
		/// </summary>
		/// <param name="tracer">
		/// The tracer.
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
		void Append(Tracer tracer, TraceLevel traceLevel, string message, Exception exception);

		#endregion
	}
}