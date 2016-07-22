// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpTraceWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{

    /// <summary>
    /// An <see cref="ITraceWriter" /> that doesn't write any <see cref="TraceEntry" />s.
    /// </summary>
    /// <remarks>
    /// This class is useful when logging is disabled.
    /// </remarks>
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

        public bool IsSynchronized { get { return true; } }

    }

}
