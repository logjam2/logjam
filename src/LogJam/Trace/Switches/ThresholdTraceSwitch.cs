// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThresholdTraceSwitch.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Switches
{
    /// <summary>
    /// An <see cref="ITraceSwitch" /> that allows <see cref="Tracer" /> events that equal or exceed a fixed threshold.
    /// </summary>
    public sealed class ThresholdTraceSwitch : ITraceSwitch
    {
        #region Fields

        private TraceLevel _threshold;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdTraceSwitch" /> class.
        /// </summary>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        public ThresholdTraceSwitch(TraceLevel threshold)
        {
            _threshold = threshold;
        }

        #endregion

        public TraceLevel Threshold { get { return _threshold; } set { _threshold = value; } }

        #region Public Methods and Operators

        /// <inheritdoc />
        public bool IsEnabled(string tracerName, TraceLevel traceLevel)
        {
            return traceLevel >= _threshold;
        }

        #endregion
    }
}
