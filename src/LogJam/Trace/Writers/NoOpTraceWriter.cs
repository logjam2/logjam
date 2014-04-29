// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Writers
{

	/// <summary>
	/// An <see cref="ITraceWriter"/> that does nothing.
	/// </summary>
	public sealed class NoOpTraceWriter : ITraceWriter
	{

		public bool IsActive { get { return false; } }

		public void Write(Tracer tracer, TraceLevel traceLevel, string message, object details)
		{}

	}

}
