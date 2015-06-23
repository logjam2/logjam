// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvroTraceEntryWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro.Trace
{
    using System;

    using LogJam.Trace;


    /// <summary>
    /// A custom <see cref="AvroEntryWriter{TEntry}"/> that wraps <see cref="TraceEntry"/>s with <see cref="AvroTraceEntry"/>
    /// before they are written to the Avro serializer.
    /// </summary>
    internal sealed class AvroTraceEntryWriter : AvroEntryWriter<TraceEntry>
    {

        private readonly AvroTraceEntry _entryWrapper = new AvroTraceEntry();

        public AvroTraceEntryWriter(AvroLogWriter parent)
            : base(parent)
        {}

        public override void Write(ref TraceEntry entry)
        {
            _entryWrapper.traceEntry = entry;
            WriteAvroLogEntry(_entryWrapper);
        }

        public override Type LogEntryType { get { return typeof(AvroTraceEntry); } }

    }

}