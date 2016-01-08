// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEntry.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;

    using LogJam.Trace.Format;
    using LogJam.Writer.Text;


    /// <summary>
    /// Holds a trace message and associated metadata in memory.
    /// </summary>
    [DefaultFormatter(typeof(DefaultTraceFormatter))]
    public struct TraceEntry : ILogEntry
    {

        /// <summary>
        /// When the trace entry was created, using UTC timezone.
        /// </summary>
        public readonly DateTime TimestampUtc;

        /// <summary>
        /// The <see cref="Tracer.Name" /> that was the source of this <c>TraceEntry</c>.
        /// </summary>
        public readonly string TracerName;

        /// <summary>
        /// The <see cref="TraceLevel" /> for this <c>TraceEntry</c>.
        /// </summary>
        public readonly TraceLevel TraceLevel;

        /// <summary>
        /// The trace message - may be multiline.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Additional trace message metadata - eg an <see cref="Exception" />.
        /// </summary>
        public readonly object Details;

        public override string ToString()
        {
            return string.Format("{0,-7}  {1,-50}  {2}", TraceLevel, TracerName, Message);
        }

        public TraceEntry(string tracerName, TraceLevel traceLevel, string message, object details = null)
        {
            TimestampUtc = DateTime.UtcNow;
            TracerName = tracerName;
            TraceLevel = traceLevel;
            Message = message;
            Details = details;
        }

    }

}
