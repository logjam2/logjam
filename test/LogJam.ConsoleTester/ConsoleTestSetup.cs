// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestSetup.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.ConsoleTester
{
    using System;

    using LogJam.Config;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;


    /// <summary>
    /// Configuration methods for console testing.
    /// </summary>
    public sealed class ConsoleTestSetup
    {

        private ConsoleLogWriterConfig _consoleLogWriterConfig;

        public ConsoleTestSetup(TraceManager traceManager)
        {
            TraceManager = traceManager;
            TraceManager.Config.TraceFirstChanceExceptions = true;
        }

        public TraceManager TraceManager { get; }

        public ConsoleLogWriterConfig ConsoleLogWriterConfig
        {
            get
            {
                if (_consoleLogWriterConfig == null)
                {
                    throw new Exception("A *Config* method must be called before ConsoleLogWriterConfig property can be accessed.");
                }
                return _consoleLogWriterConfig;
            }
            set { _consoleLogWriterConfig = value; }
        }

        /// <summary>
        /// Configures console tracing using object graph configuration (newing the config objects).
        /// </summary>
        public void ObjectGraphConfigForTrace()
        {
            ConsoleLogWriterConfig = new ConsoleLogWriterConfig();
            ConsoleLogWriterConfig.Format(new DefaultTraceFormatter());
            TraceManager.Config.Add(new TraceWriterConfig(ConsoleLogWriterConfig, TraceManagerConfig.CreateDefaultSwitchSet()));
        }

        /// <summary>
        /// Configures console tracing using object graph configuration (newing the config objects), and enables all trace levels
        /// for the <see cref="ConsoleTestCases" /> class.
        /// </summary>
        public void ObjectGraphConfigForTraceEnableAllLevels()
        {
            ConsoleLogWriterConfig = new ConsoleLogWriterConfig();
            ConsoleLogWriterConfig.Format<TraceEntry>();
            var config = new TraceWriterConfig(ConsoleLogWriterConfig)
                         {
                             Switches =
                             {
                                 // Default threshold (info) for all tracers
                                 { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) },
                                 // Enable all trace levels for ConsoleTestCases
                                 { typeof(ConsoleTestCases), new OnOffTraceSwitch(true) }
                             }
                         };
            TraceManager.Config.Add(config);
        }

        /// <summary>
        /// Fluent version of <see cref="ObjectGraphConfigForTrace" />.
        /// </summary>
        public void FluentConfigForTrace()
        {
            var traceWriterConfig = TraceManager.Config.TraceToConsole();

            // Normally you don't have to extract the ConsoleLogWriterConfig, but for the purposes of this class we do
            ConsoleLogWriterConfig = traceWriterConfig.LogWriterConfig as ConsoleLogWriterConfig;
        }

        /// <summary>
        /// Fluent version of <see cref="ObjectGraphConfigForTraceEnableAllLevels" />.
        /// </summary>
        public void FluentConfigForTraceEnableAllLevels()
        {
            var traceWriterConfig = TraceManager.Config.TraceToConsole(new SwitchSet()
                                                                {
                                                                    // Default threshold (info) for all tracers
                                                                    { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) },
                                                                    // Enable all trace levels for ConsoleTestCases
                                                                    { typeof(ConsoleTestCases), new OnOffTraceSwitch(true) }
                                                                });

            // Normally you don't have to extract the ConsoleLogWriterConfig, but for the purposes of this class we do
            ConsoleLogWriterConfig = traceWriterConfig.LogWriterConfig as ConsoleLogWriterConfig;
        }

        public void TraceTimestamps(bool includeTimestamp)
        {
            ConsoleLogWriterConfig.Format(new DefaultTraceFormatter()
                                          {
                                              IncludeTimestamp = includeTimestamp
                                          });
        }

    }

}
