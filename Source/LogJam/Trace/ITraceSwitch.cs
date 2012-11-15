// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="ITraceSwitch.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace
{
	/// <summary>
	/// Interface for determining whether or not a trace message or activity should be logged.
	/// </summary>
	public interface ITraceSwitch
	{
		#region Public Methods and Operators

		/// <summary>
		/// The is message enabled.
		/// </summary>
		/// <param name="tracer">
		/// The tracer.
		/// </param>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		bool IsEnabled(Tracer tracer, TraceLevel traceLevel);

		#endregion
	}
}