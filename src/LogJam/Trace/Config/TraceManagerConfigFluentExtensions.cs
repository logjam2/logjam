// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigFluentExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    using LogJam.Config;
    using LogJam.Util;
    using LogJam.Writer;
    using LogJam.Writer.Text;


    /// <summary>
    /// Extension methods for fluent configuration of <see cref="TraceManagerConfig" />.
    /// </summary>
    public static class TraceManagerConfigFluentExtensions
    {

        /// <summary>
        /// Trace to the text logwriter configured by <paramref name="textLogWriterConfig" />.
        /// </summary>
        /// <param name="traceManagerConfig"></param>
        /// <param name="textLogWriterConfig"></param>
        /// <param name="switchSet"></param>
        /// <param name="traceFormatter"></param>
        /// <returns></returns>
        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                TextLogWriterConfig textLogWriterConfig,
                                                SwitchSet switchSet,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(textLogWriterConfig != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(textLogWriterConfig, switchSet);
            textLogWriterConfig.Format(traceFormatter);
            traceManagerConfig.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        /// <summary>
        /// Trace to the logwriter configured by <paramref name="logWriterConfig" />.
        /// </summary>
        /// <param name="traceManagerConfig"></param>
        /// <param name="textLogWriterConfig"></param>
        /// <param name="tracerName"></param>
        /// <param name="traceSwitch"></param>
        /// <param name="traceFormatter"></param>
        /// <returns></returns>
        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                TextLogWriterConfig textLogWriterConfig,
                                                string tracerName = Tracer.All,
                                                ITraceSwitch traceSwitch = null,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(textLogWriterConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceTo(traceManagerConfig, textLogWriterConfig, CreateSwitchSet(tracerName, traceSwitch), traceFormatter);
        }

        /// <summary>
        /// Trace to the logwriter configured by <paramref name="logWriterConfig" />.
        /// </summary>
        /// <param name="traceManagerConfig"></param>
        /// <param name="logWriterConfig"></param>
        /// <param name="switchSet"></param>
        /// <returns></returns>
        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig, LogWriterConfig logWriterConfig, SwitchSet switchSet)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(logWriterConfig != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var traceWriterConfig = new TraceWriterConfig(logWriterConfig, switchSet);
            traceManagerConfig.Writers.Add(traceWriterConfig);
            return traceWriterConfig;
        }

        /// <summary>
        /// Trace to the logwriter configured by <paramref name="logWriterConfig" />.
        /// </summary>
        /// <param name="traceManagerConfig"></param>
        /// <param name="logWriterConfig"></param>
        /// <param name="tracerName"></param>
        /// <param name="traceSwitch"></param>
        /// <returns></returns>
        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                LogWriterConfig logWriterConfig,
                                                string tracerName = Tracer.All,
                                                ITraceSwitch traceSwitch = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(logWriterConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceTo(traceManagerConfig, logWriterConfig, CreateSwitchSet(tracerName, traceSwitch));
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                TextWriter textWriter,
                                                SwitchSet switchSet,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(textWriter != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            return TraceTo(traceManagerConfig, new TextWriterLogWriterConfig(textWriter), switchSet, traceFormatter);
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                Func<TextWriter> createTextWriterFunc,
                                                SwitchSet switchSet,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(createTextWriterFunc != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            return TraceTo(traceManagerConfig, new TextWriterLogWriterConfig(createTextWriterFunc), switchSet, traceFormatter);
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                TextWriter textWriter,
                                                string tracerName = Tracer.All,
                                                ITraceSwitch traceSwitch = null,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceTo(traceManagerConfig, textWriter, CreateSwitchSet(tracerName, traceSwitch), traceFormatter);
        }

        public static TraceWriterConfig TraceTo(this TraceManagerConfig traceManagerConfig,
                                                Func<TextWriter> createTextWriterFunc,
                                                string tracerName = Tracer.All,
                                                ITraceSwitch traceSwitch = null,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceTo(traceManagerConfig, createTextWriterFunc, CreateSwitchSet(tracerName, traceSwitch), traceFormatter);
        }

        /// <summary>
        /// Enables sending trace messages to all specified <paramref name="logWriterConfigs" />. This method can be called
        /// multiple times
        /// to specify different switch settings for different logWriters.
        /// </summary>
        /// <param name="traceManagerConfig"></param>
        /// <param name="logWriterConfigs"></param>
        /// <param name="switches"></param>
        /// <param name="traceFormatter"></param>
        /// <returns></returns>
        public static void TraceTo(this TraceManagerConfig traceManagerConfig,
                                   IEnumerable<ILogWriterConfig> logWriterConfigs,
                                   SwitchSet switches = null,
                                   EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(logWriterConfigs != null);

            if (switches == null)
            {
                switches = TraceManagerConfig.CreateDefaultSwitchSet();
            }

            logWriterConfigs.FormatAll(traceFormatter);
            foreach (var logWriterConfig in logWriterConfigs)
            {
                traceManagerConfig.Writers.Add(new TraceWriterConfig(logWriterConfig, switches));
            }
        }

        /// <summary>
        /// Enables sending trace messages to all configured log writers.
        /// </summary>
        /// <param name="traceManager"></param>
        /// <param name="switches"></param>
        /// <param name="traceFormatter"></param>
        /// <returns></returns>
        public static void TraceToAllLogWriters(this TraceManagerConfig config,
                                                SwitchSet switches = null,
                                                EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);

            TraceTo(config, config.LogManagerConfig.Writers, switches, traceFormatter);
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

            return UseLogWriter(config, logWriter, CreateSwitchSet(type.GetCSharpName(), traceSwitch));
        }

        public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter logWriter, string tracerName = Tracer.All, ITraceSwitch traceSwitch = null)
        {
            Contract.Requires<ArgumentNullException>(config != null);
            Contract.Requires<ArgumentNullException>(logWriter != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return UseLogWriter(config, logWriter, CreateSwitchSet(tracerName, traceSwitch));
        }

        public static TraceWriterConfig TraceToConsole(this TraceManagerConfig traceManagerConfig, SwitchSet switchSet, EntryFormatter<TraceEntry> traceFormatter = null, bool colorize = true)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            ConsoleLogWriterConfig consoleLogWriterConfig = traceManagerConfig.LogManagerConfig.UseConsole(colorize);
            return TraceTo(traceManagerConfig, consoleLogWriterConfig, switchSet, traceFormatter);
        }

        public static TraceWriterConfig TraceToConsole(this TraceManagerConfig traceManagerConfig,
                                                       string tracerName = Tracer.All,
                                                       ITraceSwitch traceSwitch = null,
                                                       EntryFormatter<TraceEntry> traceFormatter = null,
                                                       bool colorize = true)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceToConsole(traceManagerConfig, CreateSwitchSet(tracerName, traceSwitch), traceFormatter, colorize);
        }

        public static TraceWriterConfig TraceToDebugger(this TraceManagerConfig traceManagerConfig, SwitchSet switchSet, EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            DebuggerLogWriterConfig debuggerLogWriterConfig = traceManagerConfig.LogManagerConfig.UseDebugger();
            return TraceTo(traceManagerConfig, debuggerLogWriterConfig, switchSet, traceFormatter);
        }

        public static TraceWriterConfig TraceToDebugger(this TraceManagerConfig traceManagerConfig,
                                                        string tracerName = Tracer.All,
                                                        ITraceSwitch traceSwitch = null,
                                                        EntryFormatter<TraceEntry> traceFormatter = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceToDebugger(traceManagerConfig, CreateSwitchSet(tracerName, traceSwitch), traceFormatter);
        }

        public static TraceWriterConfig TraceToList(this TraceManagerConfig traceManagerConfig, IList<TraceEntry> list, SwitchSet switchSet)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(list != null);
            Contract.Requires<ArgumentNullException>(switchSet != null);

            var listLogWriterConfig = new ListLogWriterConfig<TraceEntry>()
                                      {
                                          List = list
                                      };
            return TraceTo(traceManagerConfig, listLogWriterConfig, switchSet);
        }

        public static TraceWriterConfig TraceToList(this TraceManagerConfig traceManagerConfig,
                                                    IList<TraceEntry> list,
                                                    string tracerName = Tracer.All,
                                                    ITraceSwitch traceSwitch = null)
        {
            Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
            Contract.Requires<ArgumentNullException>(list != null);
            Contract.Requires<ArgumentNullException>(tracerName != null);

            return TraceToList(traceManagerConfig, list, CreateSwitchSet(tracerName, traceSwitch));
        }

        private static SwitchSet CreateSwitchSet(string rootTracerName, ITraceSwitch traceSwitch = null)
        {
            if (rootTracerName == null)
            {
                rootTracerName = Tracer.All;
            }
            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }
            return new SwitchSet()
                   {
                       { rootTracerName, traceSwitch }
                   };
        }

    }

}
