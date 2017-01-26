// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using LogJam.Config.Json;
    using LogJam.Trace;
    using LogJam.Writer.Text;


    /// <summary>
    /// Configures a log writer that writes to the debugger window.
    /// </summary>
    [JsonTypeHint("Target", "Debugger")]
    public sealed class DebuggerLogWriterConfig : TextLogWriterConfig
    {

        /// <summary>
        /// Creates a new <see cref="DebuggerLogWriterConfig" />.
        /// </summary>
        public DebuggerLogWriterConfig()
        {}

        protected override FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory)
        {
            return new DebuggerFormatWriter(setupTracerFactory);
        }

        /// <summary>
        /// Equals and <see cref="GetHashCode"/> are overridden so no more than a single DebuggerLogWriterConfig instance
        /// will be stored in <see cref="LogManager.Config"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is DebuggerLogWriterConfig);
        }

        public override int GetHashCode()
        {
            // All instances of DebuggerLogWriterConfig have the same hash code, and are equal.
            return GetType().GetHashCode();
        }

    }

}
