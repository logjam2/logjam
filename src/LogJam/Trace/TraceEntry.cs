// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEntry.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
	using System;


	/// <summary>
	/// Holds a trace message and associated metadata in memory.
	/// </summary>
	public struct TraceEntry : ILogEntry
	{
		/// <summary>
		/// When the trace entry was created, using UTC timezone.
		/// </summary>
		public DateTime TimestampUtc;

		/// <summary>
		/// The <see cref="Tracer.Name"/> that was the source of this <c>TraceEntry</c>.
		/// </summary>
		public string TracerName;

		/// <summary>
		/// The <see cref="TraceLevel"/> for this <c>TraceEntry</c>.
		/// </summary>
		public TraceLevel TraceLevel;

		/// <summary>
		/// The trace message - may be multiline.
		/// </summary>
		public string Message;

		/// <summary>
		/// Additional trace message metadata - eg an <see cref="Exception"/>.
		/// </summary>
		public object Details;

		public override string ToString()
		{
			return string.Format("{0,-7}  {1,-50}  {2}", TraceLevel, TracerName, Message);
		}

	}

}