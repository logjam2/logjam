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

	using LogJam.Schema;


    /// <summary>
	/// Holds a trace message and associated metadata in memory.
	/// </summary>
	public struct TraceEntry : ITimestampedLogEntry
	{
		/// <summary>
		/// When the trace entry was created, using UTC timezone.
		/// </summary>
        public readonly DateTime TimestampUtc;

		/// <summary>
		/// The <see cref="Tracer.Name"/> that was the source of this <c>TraceEntry</c>.
		/// </summary>
		[Required]
        public readonly string TracerName;

		/// <summary>
		/// The <see cref="TraceLevel"/> for this <c>TraceEntry</c>.
		/// </summary>
        public readonly TraceLevel TraceLevel;

		/// <summary>
		/// The trace message - may be multiline.
		/// </summary>
        [Required]
        public readonly string Message;

		/// <summary>
		/// Additional trace information - eg additional multi-line text.
		/// </summary>
        public readonly string Details;

        /// <summary>
        /// An <see cref="Exception"/>.
        /// </summary>
        public readonly Exception Exception;

        public TraceEntry(string tracerName, TraceLevel traceLevel, string message, string details = null)
        {
            TimestampUtc = DateTime.UtcNow;
            TracerName = tracerName;
            TraceLevel = traceLevel;
            Message = message;
            Details = details;
            Exception = null;
        }

        public TraceEntry(string tracerName, TraceLevel traceLevel, string message, Exception exception, string details = null)
        {
            TimestampUtc = DateTime.UtcNow;
            TracerName = tracerName;
            TraceLevel = traceLevel;
            Message = message;
            Details = details;
            Exception = exception;
        }

        public override string ToString()
		{
			return string.Format("{0,-7}\t{1,-50}\t{2}", TraceLevel, TracerName, Message);
		}

	    DateTime ITimestampedLogEntry.TimestampUtc { get { return this.TimestampUtc; } }
	}

}