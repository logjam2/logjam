// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigFluentExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using LogJam.Config;
    using LogJam.Trace.Format;
    using LogJam.Writer;
    using LogJam.Writer.Text;


    /// <summary>
    /// Extension methods for fluent configuration of <see cref="TraceManagerConfig" />.
    /// </summary>
    public static class TraceManagerConfigFluentExtensions
    {

        /// <summary>
        /// Trace to the logwriter configured by <paramref name="logWriterConfig" />.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logWriterConfig"></param>
        /// <param name="switchSet"></param>
        /// <returns></returns>
        public static TraceWriterConfig TraceTo(this TraceManagerConfig config, LogWriterConfig logWriterConfig, SwitchSet switchSet)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(logWriterConfig != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(logWriterConfig, switchSet);
            config.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        /// <summary>
        /// Trace to the logwriter configured by <paramref name="logWriterConfig" />.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logWriterConfig"></param>
        /// <param name="tracerName"></param>
        /// <param name="traceSwitch"></param>
        /// <returns></returns>
        public static TraceWriterConfig TraceTo(this TraceManagerConfig config, LogWriterConfig logWriterConfig, string tracerName = Tracer.All, ITraceSwitch traceSwitch = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(logWriterConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { tracerName, traceSwitch }
                           };
            return TraceTo(config, logWriterConfig, switches);
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig config, TextWriter textWriter, SwitchSet switchSet, EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(new TextWriterLogWriterConfig(textWriter).Format(traceFormatter), switchSet);
            config.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig config, Func<TextWriter> createTextWriterFunc, SwitchSet switchSet, EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(new TextWriterLogWriterConfig(createTextWriterFunc).Format(traceFormatter), switchSet);
            config.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig config,
                                                TextWriter textWriter,
                                                string tracerName = Tracer.All,
                                                ITraceSwitch traceSwitch = null,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { tracerName, traceSwitch }
                           };
            return TraceTo(config, textWriter, switches, traceFormatter);
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig config,
                                                Func<TextWriter> createTextWriterFunc,
                                                string tracerName = Tracer.All,
                                                ITraceSwitch traceSwitch = null,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { tracerName, traceSwitch }
                           };
            return TraceTo(config, createTextWriterFunc, switches, traceFormatter);
        }

        /// <summary>
        /// Use an existing <paramref name="logWriter" /> along with the specified <paramref name="switchSet" />.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logWriter"></param>
        /// <param name="switchSet"></param>
        public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter logWriter, SwitchSet switchSet)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(logWriter != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(logWriter, switchSet);
            config.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter logWriter, Type type, ITraceSwitch traceSwitch = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(logWriter != null);
            Contract.Requires<ArgumentNullException>(type != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { type, traceSwitch }
                           };
            return UseLogWriter(config, logWriter, switches);
        }

        public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter logWriter, string tracerName = Tracer.All, ITraceSwitch traceSwitch = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(logWriter != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { tracerName, traceSwitch }
                           };
            return UseLogWriter(config, logWriter, switches);
        }

        public static TraceWriterConfig TraceToConsole(this TraceManagerConfig config, SwitchSet switchSet, EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(new ConsoleLogWriterConfig().Format(traceFormatter), switchSet);
            config.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        public static TraceWriterConfig TraceToConsole(this TraceManagerConfig config,
                                                       string tracerName = Tracer.All,
                                                       ITraceSwitch traceSwitch = null,
                                                       EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { tracerName, traceSwitch }
                           };
            return TraceToConsole(config, switches, traceFormatter);
        }

        public static TraceWriterConfig TraceToDebugger(this TraceManagerConfig config, SwitchSet switchSet, EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(new DebuggerLogWriterConfig().Format(traceFormatter), switchSet);
            config.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        public static TraceWriterConfig TraceToDebugger(this TraceManagerConfig config,
                                                        string tracerName = Tracer.All,
                                                        ITraceSwitch traceSwitch = null,
                                                        EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            var switches = new SwitchSet()
                           {
                               { tracerName, traceSwitch }
                           };
            return TraceToDebugger(config, switches, traceFormatter);
        }

    }

}
