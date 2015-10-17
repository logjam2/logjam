// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerLogWriterConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using LogJam.Config.Json;
    using LogJam.Trace;
    using LogJam.Writer;
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
        {
            // Default Synchronized to false
            Synchronized = false;
        }

        public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            var writer = new DebuggerFormatWriter(setupTracerFactory);
            ApplyConfiguredFormatters(writer);
            return writer;
        }

    }

}
