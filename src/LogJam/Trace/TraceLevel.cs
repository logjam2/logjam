// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceLevel.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    //using Newtonsoft.Json;
    //using Newtonsoft.Json.Converters;

    /// <summary>
    /// The different types of trace messages that can be logged.
    /// </summary>
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum TraceLevel : byte
    {

        /// <summary>
        /// Debug trace level - containing comprehensive information for debugging. This level should
        /// only be enabled when debugging a problem due to the amount of log data and potential for
        /// performance degradation due to high volume logging.
        /// </summary>
        Debug,

        /// <summary>
        /// Verbose trace level - containing detailed information. Verbose messages should be disabled when a
        /// system is stable and running at high volume in production, b/c the log volume can be excessive.
        /// </summary>
        Verbose,

        /// <summary>
        /// Informational trace level - containing significant informational events that should be logged in production.
        /// In production systems, the default is that <c>Info</c>, <see cref="Warn" />, <see cref="Error" />, and
        /// <see cref="Severe" />
        /// messages are logged.
        /// <c>Info</c> events are not errors.
        /// </summary>
        Info,

        /// <summary>
        /// Warning trace level - containing warning messages, which may or may not be errors, or which may be minor errors.
        /// </summary>
        Warn,

        /// <summary>
        /// Error trace level - indicating significant errors in operation.
        /// </summary>
        Error,

        /// <summary>
        /// Severe error - typically used for severe or critical errors.
        /// </summary>
        Severe,

        /// <summary>
        /// No trace output is propagated.
        /// </summary>
        Off

    }
}
