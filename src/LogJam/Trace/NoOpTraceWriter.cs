// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
	using System;


	/// <summary>
	/// An <see cref="ITraceWriter"/> that doesn't write any <see cref="TraceEntry"/>s.
	/// </summary>
	/// <remarks>
	/// This class is useful when logging is disabled.</remarks>
	internal sealed class NoOpTraceWriter : ITraceWriter
	{

		public void Write(ref TraceEntry entry)
		{}

		public bool IsTraceEnabled(string tracerName, TraceLevel traceLevel)
		{
			return false;
		}

		public TraceWriter[] ToTraceWriterArray()
		{
			return new TraceWriter[0];
		}

		public void Dispose()
		{}

		public bool IsEnabled { get { return false; } }

		public Type LogEntryType { get { return typeof(TraceEntry); } }

		public bool IsSynchronized { get { return true; } }

	}

}