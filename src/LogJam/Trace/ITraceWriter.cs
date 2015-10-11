// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITraceWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using LogJam.Writer;


    /// <summary>
    /// Specializes <see cref="IEntryWriter{TEntry}" /> for writing <see cref="TraceEntry" />s.
    /// </summary>
    internal interface ITraceWriter : IEntryWriter<TraceEntry>
    {

        /// <summary>
        /// Returns <c>true</c> if a trace message for the specified <paramref name="tracerName" /> and
        /// <paramref name="traceLevel" /> should
        /// be written to this <see cref="ITraceWriter" />.
        /// </summary>
        /// <param name="tracerName">The <see cref="Tracer.Name" /> for a <see cref="Tracer" /> that received a <c>Tracer</c> call.</param>
        /// <param name="traceLevel">The <see cref="TraceLevel" /> for a <see cref="Tracer" /> call.</param>
        /// <returns></returns>
        bool IsTraceEnabled(string tracerName, TraceLevel traceLevel);

        /// <summary>
        /// Converts the <see cref="ITraceWriter" /> to the array of <see cref="TraceWriter" />s that it represents.
        /// </summary>
        /// <returns></returns>
        TraceWriter[] ToTraceWriterArray();

    }

}
