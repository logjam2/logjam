// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnOffTraceSwitch.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Switches
{

    /// <summary>
    /// An <see cref="ITraceSwitch" /> that either allows all <see cref="Tracer" /> calls, or blocks all <c>Tracer</c> calls,
    /// from being logged.
    /// </summary>
    public sealed class OnOffTraceSwitch : ITraceSwitch
    {

        private bool _on;

        /// <summary>
        /// Creates a new <see cref="OnOffTraceSwitch" />
        /// </summary>
        /// <param name="on"><c>true</c> to allow all <see cref="Tracer" /> calls to be logged.</param>
        public OnOffTraceSwitch(bool on)
        {
            _on = on;
        }

        public bool On { get { return _on; } set { _on = value; } }

        public bool IsEnabled(string tracerName, TraceLevel traceLevel)
        {
            return _on;
        }

    }

}
