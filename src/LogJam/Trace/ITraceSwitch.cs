// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITraceSwitch.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    /// <summary>
    /// Interface for determining whether or not a trace message or activity should be logged.
    /// </summary>
    public interface ITraceSwitch
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns <c>true</c> if a trace message for <paramref name="tracerName" />, in the current trace context (time, thread),
        /// with <see cref="TraceLevel" /> equal to <paramref name="traceLevel" />, should be logged.
        /// </summary>
        /// <param name="tracerName">A <see cref="Tracer.Name" />.</param>
        /// <param name="traceLevel">
        /// The trace level.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        bool IsEnabled(string tracerName, TraceLevel traceLevel);

        #endregion
    }
}
