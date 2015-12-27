// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestSetup.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.ConsoleTester
{
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

        public ConsoleTestSetup(TraceManager traceManager)
        {
            TraceManager = traceManager;
        }

        public TraceManager TraceManager { get; set; }
        public ConsoleLogWriterConfig ConsoleLogWriterConfig { get; set; }

        /// <summary>
        /// Configures console tracing using object graph configuration (newing the config objects).
        /// </summary>
        public void ObjectGraphConfigForTrace()
        {
            ConsoleLogWriterConfig = new ConsoleLogWriterConfig();
            ConsoleLogWriterConfig.Format(new DefaultTraceFormatter());
            TraceManager.Config.Writers.Add(new TraceWriterConfig(ConsoleLogWriterConfig, TraceManagerConfig.CreateDefaultSwitchSet()));
        }

        /// <summary>
        /// Configures console tracing using object graph configuration (newing the config objects), and enables all trace levels for the <see cref="ConsoleTestCases"/> class.
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
            TraceManager.Config.Writers.Add(config);
        }

        /// <summary>
        /// Fluent version of <see cref="ObjectGraphConfigForTrace"/>.
        /// </summary>
        public void FluentConfigForTrace()
        {
            var traceWriterConfig = TraceManager.Config.TraceToConsole();

            // Normally you don't have to extract the ConsoleLogWriterConfig, but for the purposes of this class we do
            ConsoleLogWriterConfig = traceWriterConfig.LogWriterConfig as ConsoleLogWriterConfig;
        }

        /// <summary>
        /// Fluent version of <see cref="ObjectGraphConfigForTraceEnableAllLevels"/>.
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

        public void TraceTimestamps()
        {
            ConsoleLogWriterConfig.Format(new DefaultTraceFormatter()
                                          {
                                              IncludeTimestamp = true
                                          });
        }

    }

}