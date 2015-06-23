// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvroTraceEntry.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro.Trace
{
    using System;
    using System.Runtime.Serialization;

    using LogJam.Schema;
    using LogJam.Trace;


    /// <summary>
    /// Wraps <see cref="TraceEntry"/> to customize Avro serialization.
    /// </summary>
    /// <remarks>
    /// This class provides the shape of <see cref="TraceEntry"/> objects that are logged to Avro.
    /// </remarks>
    [SerializedTypeName("TraceEntry", AvroSchemaNamespace)]
    public sealed class AvroTraceEntry : ITimestampedLogEntry
    {

        internal const string AvroSchemaNamespace = "LogJam.Trace";

        private AvroExceptionInfo _exceptionInfo;

        /// <summary>
        /// The current wrapped <see cref="TraceEntry"/>.  Can be repeatedly overwritten for subsequent writes.
        /// </summary>
        [IgnoreDataMember]
        public TraceEntry traceEntry;

        /// <summary>
        /// When the trace entry was created, using UTC timezone.
        /// </summary>
        public DateTime TimestampUtc { get { return traceEntry.TimestampUtc; } }

        /// <summary>
        /// The <see cref="Tracer.Name"/> that was the source of this <c>TraceEntry</c>.
        /// </summary>
        [Required]
        public string TracerName { get { return traceEntry.TracerName; } }

        /// <summary>
        /// The <see cref="TraceLevel"/> for this <c>TraceEntry</c>.
        /// </summary>
        public TraceLevel TraceLevel { get { return traceEntry.TraceLevel; } }

        /// <summary>
        /// The trace message - may be multiline.
        /// </summary>
        [Required]
        public string Message { get { return traceEntry.Message; } }

        /// <summary>
        /// Additional trace information - eg additional multi-line text.
        /// </summary>
        public string Details { get { return traceEntry.Details; } }

        /// <summary>
        /// Structured info about an <see cref="Exception"/>.
        /// </summary>
        public AvroExceptionInfo Exception
        {
            get
            {
                if (traceEntry.Exception == null)
                {
                    return null;
                }
                if (_exceptionInfo == null)
                {
                    _exceptionInfo = new AvroExceptionInfo();
                }
                _exceptionInfo.SetException(traceEntry.Exception);
                return _exceptionInfo;
            }
        }

    }

}